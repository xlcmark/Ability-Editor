using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(ObstacleController))]
public class MovementController : MonoBehaviour
{
    public enum ComandState { Stop, Move, Attack, AttackSomone, Spelling, SpellPrepare, Channel, Uncontroller }//attacksomeone強制鎖定目標//attack:A點地
    public ComandState CurState;
    NavMeshAgent agent;
    EventControler ec;
    ObstacleController obstacleController;
    UnitBehaviour ub;
    public Unit unit { private set; get; }
    DamageSystem damageSystem;
    DamageSystem targetDs;
    AbilitySystem abilitySystem;
    Animator anim;

    public ParticleSystem stunFx;
    public string hitVfx;
    public string criticalHitVfx;
    [Tooltip("遠程單位")]
    public bool IsRemoteUnit;
    [ConditionHide("IsRemoteUnit", true)]
    public BaseAttTrackingProjectile OriginProjectile;
    private BaseAttTrackingProjectile CurProjectile;
    public Transform firePoint;
    public Unit target { private set; get; }
    public Vector3 attackOnPos { private set; get; }//A點地的位置
    private float nextAttackTime;
    private Ability spellingAbility;
    private EventParameter eventParameter;
    private bool IsLockState;

    public class LastMove//施法完回到上一動
    {
        public ComandState state;
        public Vector3 pos;
        public Unit targetUnit;
        public bool IsDirty;//是否用過
        public LastMove(ComandState _state, Vector3 _pos, Unit _unit)
        {
            state = _state;
            pos = _pos;
            targetUnit = _unit;
            IsDirty = false;
        }
    }
    private LastMove lastMove;

    public event System.Action OnUncontrollable;
    public event System.Action OnControllable;


    private void Awake()
    {
        obstacleController = GetComponent<ObstacleController>();
        agent = GetComponent<NavMeshAgent>();
        ub = GetComponent<UnitBehaviour>();
        damageSystem = GetComponent<DamageSystem>();
        unit = GetComponent<Unit>();
        unit.OnInit += Unit_OnInit;
        unit.OnPropertyChanged += Unit_OnPropertyChanged;
        unit.OnStopMove += Unit_OnStopUnit;
        unit.OnActiveMove += Unit_OnActiveUnit;
        unit.OnDead += Unit_OnDead;
        unit.OnBaseProjectileChanged += ChangedProjectile;
        anim = GetComponent<Animator>();
        abilitySystem = GetComponent<AbilitySystem>();
        stunFx = Instantiate(stunFx, transform);
        stunFx.gameObject.SetActive(false);
        ec = GetComponent<EventControler>();

        if (OriginProjectile != null)
        {
            CurProjectile = OriginProjectile;
            //create projectile pool
            GM.Instance.projectilePool.CreatePool(OriginProjectile, 2);
        }
    }

    private void Update()
    {
        if (target != null && !target.isActiveAndEnabled) target = null;//對象關閉代表死亡
        if (target != null && !target.GetTeamVision(unit.team)) target = null;//對象突然失去視野

        switch (CurState)
        {
            case ComandState.Move:
                if (CheckArriavlDestination())
                {
                    ChangeState(ComandState.Stop, Vector3.zero, null);
                }

                break;
            case ComandState.Stop:
                if (ub != null)
                {
                    ub.FindTarget(this);
                }
                break;
            case ComandState.Attack:
                if (ub != null)
                {
                    ub.FindTarget(this);
                }
                if (target != null)
                {
                    FacingTarget();
                    if (AttackOnTarget())//check range
                    {
                        obstacleController.StopAndObstacle();
                        if (Time.time >= nextAttackTime)//check attack interval
                        {
                            anim.SetTrigger("attack");
                            anim.SetFloat("atkType", Random.Range(0, 100) < unit.CriticalStrike.FinalValue ? 1 : 0);//爆擊檢定
                            nextAttackTime = Time.time + 1 / unit.AttackSpeed.FinalValue;
                            //event attack start
                            EventParameter e = new EventParameter();
                            e.target = target.transform;
                            ec.OnEvent(EventType.OnAttackStart, e);
                        }
                    }
                    else
                    {
                        obstacleController.SetDestination(target.transform.position);
                    }
                    if (ub != null)
                    {
                        ub.CheckPatience(this);
                    }
                }
                else//無對象
                {
                    if ((agent.destination - attackOnPos).sqrMagnitude > 1)//若還沒到目的地
                    {
                        MoveToPoint(attackOnPos);
                    }
                    else if (CheckArriavlDestination())
                    {
                        ChangeState(ComandState.Stop, Vector3.zero, null);
                    }
                }
                break;
            case ComandState.AttackSomone:
                if (target != null)
                {
                    FacingTarget();
                    if (AttackOnTarget())//check range
                    {
                        obstacleController.StopAndObstacle();
                        if (Time.time >= nextAttackTime)//check attack interval
                        {
                            anim.SetTrigger("attack");//attack
                            anim.SetFloat("atkType", Random.Range(0, 100) < unit.CriticalStrike.FinalValue ? 1 : 0);//爆擊檢定
                            nextAttackTime = Time.time + 1 / unit.AttackSpeed.FinalValue;
                            //event attack start
                            EventParameter e = new EventParameter();
                            e.target = target.transform;
                            ec.OnEvent(EventType.OnAttackStart, e);
                        }
                    }
                    else//if out of range to follow target
                    {
                        obstacleController.SetDestination(target.transform.position);
                    }
                }
                else//失去目標
                {
                    ChangeState(ComandState.Stop, Vector3.zero, null);
                }
                break;
            case ComandState.SpellPrepare:
                if (target != null)
                {
                    obstacleController.SetDestination(target.transform.position);//持續追蹤
                    FacingTarget();
                }
                if (CheckSpellRange())
                {
                    ChangeState(ComandState.Spelling, Vector3.zero, null);
                }
                break;

            default:
                break;
        }
        anim.SetBool("IsMoving", agent.velocity.magnitude != 0);//移動時播放動畫

    }
    public void ChangeState(ComandState newState, Vector3 pos, Unit targetUnit)
    {
        if (IsLockState) return;
        //exit state
        switch (CurState)
        {
            case ComandState.Spelling:
                ActiveAgent();
                break;
            case ComandState.Channel:
                if (spellingAbility != null && spellingAbility.curState == Ability.AbilityState.Channell)
                {
                    spellingAbility.LockAbility();
                    spellingAbility.UnLockAbility();
                }
                break;
            case ComandState.Uncontroller:
                ActiveAgent();
                break;
            default:
                break;
        }
        CurState = newState;
        //enter state
        switch (newState)
        {
            case ComandState.Move:
                MoveToPoint(pos);
                break;
            case ComandState.Stop:
                StopFollowTarget();
                attackOnPos = transform.position;
                break;
            case ComandState.Attack:
                if (targetUnit != null)//目標
                {
                    FollowTarget(targetUnit);
                }
                else//點地
                {
                    attackOnPos = pos;
                    if (ub != null)
                    {
                        List<Unit> tarUnits = ub.SearchEnemy(this);
                        if (tarUnits.Count != 0)
                            FollowTarget(tarUnits[0]);
                    }
                }
                break;
            case ComandState.AttackSomone:
                FollowTarget(targetUnit);
                break;
            case ComandState.Spelling:
                SpellTrigger();

                if (CurState != ComandState.Spelling) return;
                IsLockState = true;
                StopAgent();
                break;
            case ComandState.Channel:
                StopFollowTarget();
                attackOnPos = transform.position;
                break;
            case ComandState.Uncontroller:
                IsLockState = true;
                StopAgent();
                break;
            default:
                break;
        }

    }
    //spellPrepare
    public void ChangeState(ComandState newState, Vector3 pos, Transform targetUnit, Ability ability)
    {
        if (IsLockState) return;
        if (newState != ComandState.SpellPrepare) return;
        if (ability.curState != Ability.AbilityState.Ready) return;//技能未準備好

        //無對象或目標地的技能 紀錄上個動作 在技能結束時返回該動作
        if ((ability.behavior & Ability.Behavior.Dont_ResumeMovement) == 0)
        {
            //攻擊或移動才要返回
            if (CurState == ComandState.Attack || CurState == ComandState.AttackSomone || CurState == ComandState.Move)
            {
                lastMove = new LastMove(CurState, agent.destination, target);
            }
        }

        CurState = newState;
        MoveToSpell(targetUnit, pos, ability);
    }

    public void MoveToPoint(Vector3 pos)
    {
        target = null;
        targetDs = null;
        agent.stoppingDistance = 0;
        agent.updateRotation = true;

        obstacleController.SetDestination(pos);
    }
    private bool CheckArriavlDestination()
    {
        if (agent.enabled == true && !agent.pathPending)
        {
            if (agent.remainingDistance <= agent.stoppingDistance)
            {
                //if (agent.velocity.sqrMagnitude == 0)
                {
                    return true;
                }
            }
        }
        return false;
    }
    private bool CheckSpellRange()
    {
        float dis = Vector3.Distance(new Vector3(transform.position.x, 0, transform.position.z), new Vector3(agent.destination.x, 0, agent.destination.z));
        if (dis <= agent.stoppingDistance)
        {
            return true;
        }
        return false;
    }

    private void FollowTarget(Unit _target)
    {
        if (_target == unit) return;//不能指定自己
        if (target == _target) return;

        agent.updateRotation = false;
        target = _target;
        targetDs = target.GetComponent<DamageSystem>();

        if (_target.team != unit.team)//敵人
        {
            agent.stoppingDistance = unit.AttackRange.FinalValue * 0.8f;//停在攻擊距離
        }
        else if (_target.team == unit.team)//友軍
        {
            float UnitRad = agent.radius;
            NavMeshAgent targetAgent = _target.GetComponent<NavMeshAgent>();
            if (targetAgent != null)
            {
                UnitRad += targetAgent.radius;
            }
            agent.stoppingDistance = UnitRad;
        }

        if (ub != null)
        {
            ub.ResetPatient();//耐心重置
        }
    }
    private void MoveToSpell(Transform _target, Vector3 pos, Ability ability)
    {
        if (spellingAbility != null)
        {
            spellingAbility.OnPreExe -= SpellingAbility_OnPreExe;
        }
        spellingAbility = ability;
        spellingAbility.OnPreExe += SpellingAbility_OnPreExe;
        eventParameter = new EventParameter { target = _target, point = pos };

        target = null;
        targetDs = null;
        //轉向
        if (pos != Vector3.zero)
        {
            Vector3 dir = pos - transform.position;
            transform.forward = dir;
        }
        //指向性技能無範圍限制
        if (ability.SpellRange == 0)
        {
            ChangeState(ComandState.Spelling, Vector3.zero, null);
            return;
        }

        agent.stoppingDistance = ability.SpellRange;
        if (_target != null)
        {
            target = _target.GetComponent<Unit>();
            agent.updateRotation = false;
        }
        else
        {
            obstacleController.SetDestination(pos);
        }
    }
    public void StopFollowTarget()
    {
        target = null;
        targetDs = null;
        agent.stoppingDistance = 0;
        agent.updateRotation = true;
        if (agent.enabled == true)
            agent.ResetPath();

        obstacleController.StopAndObstacle();
    }
    private void FacingTarget()
    {
        Vector3 dir = (target.transform.position - transform.position).normalized;
        Quaternion lookRot = Quaternion.LookRotation(new Vector3(dir.x, 0, dir.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, Time.deltaTime * 5);
    }
    private bool AttackOnTarget()
    {
        if (target.team == unit.team) return false;
        //距離大於攻擊距離return
        if (Vector3.Distance(new Vector3(transform.position.x, 0, transform.position.z), new Vector3(target.transform.position.x, 0, target.transform.position.z)) > unit.AttackRange.FinalValue)
        {
            return false;
        }

        return true;
    }

    //動畫幀
    public void CheckTargetInRange()
    {
        if (!anim.GetCurrentAnimatorStateInfo(1).IsName("empty")) return;//基層被覆蓋時，不要觸發事件
        if (target == null) return;
        if (targetDs == null) return;
        if (!target.isActiveAndEnabled) return;
        if (target.team == unit.team) return;
        if (Vector3.Distance(new Vector3(transform.position.x, 0, transform.position.z), new Vector3(target.transform.position.x, 0, target.transform.position.z)) > unit.AttackRange.FinalValue) return;
        //attack
        if (IsRemoteUnit)//遠程
            ShootProjectile();
        else//近程
            OnHit(target, targetDs);
    }

    private void ShootProjectile()
    {
        if (CurProjectile == null) return;

        var projectile = GM.Instance.projectilePool.GetObject(CurProjectile);
        projectile.transform.position = firePoint.position;
        projectile.transform.forward = transform.forward;
        projectile.SetProjectile(target, targetDs, OnHit);
    }
    private void OnHit(Unit _target, DamageSystem _targetDs)
    {
        if (_targetDs == null || _target == null) return;
        bool IsCritical = anim.GetFloat("atkType") == 1;//爆級檢定
        AttackInfo attackInfo = new AttackInfo(transform);
        int PhyDmg = Mathf.RoundToInt(unit.Attack.FinalValue);
        PhyDmg *= IsCritical ? 2 : 1;
        //basic atk bonus普攻額外傷害
        int MagDmg = 0, PureDmg = 0;
        List<string> BonusVfxs = new List<string>();
        if (unit.basicAtkBonusList.Count > 0)
        {
            foreach (var bonus in unit.basicAtkBonusList)
            {
                //vfx
                if (bonus.Value.vfxName != null)
                {
                    BonusVfxs.Add(bonus.Value.vfxName);
                }
                //value
                int bonusValue = Mathf.RoundToInt(bonus.Value.value.GetFinalValue(unit, _target, bonus.Key.RefAbility.Level));
                switch (bonus.Value.AtkType)
                {
                    case DamageType.Physics:
                        PhyDmg += bonusValue;
                        break;
                    case DamageType.Magic:
                        MagDmg += bonusValue;
                        break;
                    case DamageType.Pure:
                        PureDmg += bonusValue;
                        break;
                }
            }
        }
        //三種攻擊類型分開計算跳數字
        int realdmg = _targetDs.TakeDamage(PhyDmg, DoDamage.ValueType.Constant, attackInfo);
        if (MagDmg != 0)
            _targetDs.TakeDamage(MagDmg, DoDamage.ValueType.Constant, attackInfo);
        if (PureDmg != 0)
            _targetDs.TakeDamage(PureDmg, DoDamage.ValueType.Constant, attackInfo);
        //life steal
        int heal = Mathf.RoundToInt(realdmg * unit.LifeSteal.FinalValue * 0.01f);
        if (damageSystem != null && heal != 0)
            damageSystem.TakeHeal(heal, Heal.ValueType.Constant);

        //vfx
        Vector3 hitDir = _targetDs.transform.position - transform.position;
        hitDir.y = 0;
        if (BonusVfxs.Count > 0)
        {
            //附加攻擊的特效
            for (int i = 0; i < BonusVfxs.Count; i++)
            {
                _target.AddVFX(BonusVfxs[i], AttachEffectPoint.body, Quaternion.LookRotation(hitDir));
            }
        }
        else if (!string.IsNullOrEmpty( criticalHitVfx) && IsCritical)//爆擊特效
            _target.AddVFX(criticalHitVfx, AttachEffectPoint.body, Quaternion.LookRotation(hitDir));
        else if (!string.IsNullOrEmpty(hitVfx))//普攻特效
            _target.AddVFX(hitVfx, AttachEffectPoint.body, Quaternion.LookRotation(hitDir));
        //自動尋人
        if (ub != null)
        {
            ub.ResetPatient();
        }
        //event attack landed
        EventParameter e = new EventParameter();
        e.target = _target.transform;
        ec.OnEvent(EventType.OnAttackLanded, e);
    }
    private void SpellTrigger()
    {
        if (spellingAbility.abilityAnimation != Ability.AbilityAnimation.none)
            anim.SetTrigger(spellingAbility.abilityAnimation.ToString());
        abilitySystem.StartActiveAbility(spellingAbility, eventParameter);
    }

    void Unit_OnInit()
    {
        agent.speed = unit.MoveSpeed.FinalValue / 100;
    }

    void Unit_OnPropertyChanged()
    {
        //調整動畫速度
        anim.SetFloat("attackRate", unit.AttackSpeed.FinalValue / unit.AttackSpeed.BaseValue);
        anim.SetFloat("moveSpeed", unit.MoveSpeed.FinalValue / unit.MoveSpeed.BaseValue);

        agent.speed = unit.MoveSpeed.FinalValue / 100;
    }
    private void StopAgent()
    {
        obstacleController.StopAndObstacle();

        OnUncontrollable?.Invoke();//讓cursor不能改狀態
        if (unit.Stun > 0)//特效
        {
            anim.SetBool("IsStunning", true);
            stunFx.gameObject.SetActive(true);
        }
    }
    private void ActiveAgent()
    {

        stunFx.gameObject.SetActive(false);
        anim.SetBool("IsStunning", false);

        OnControllable?.Invoke();
    }

    private void Unit_OnStopUnit()
    {
        ChangeState(ComandState.Uncontroller, Vector3.zero, null);
    }

    private void Unit_OnActiveUnit()
    {
        if (CurState != ComandState.Uncontroller) return;
        IsLockState = false;
        ChangeState(ComandState.Stop, Vector3.zero, null);
    }

    void SpellingAbility_OnPreExe()
    {
        IsLockState = false;
        if ((spellingAbility.behavior & Ability.Behavior.Channelled) == 0)
        {
            //若有上一動 需回到上一動
            if (lastMove?.IsDirty == false)
            {
                ChangeState(lastMove.state, lastMove.pos, lastMove.targetUnit);
                lastMove.IsDirty = true;
            }
            else
            {
                ChangeState(ComandState.Stop, Vector3.zero, null);
            }
        }
        else//channel
        {
            ChangeState(ComandState.Channel, Vector3.zero, null);
        }
    }

    void Unit_OnDead(Unit _unit)
    {
        IsLockState = false;
        ChangeState(ComandState.Uncontroller, Vector3.zero, null);
    }
    public void ResetBasicAttack()
    {
        nextAttackTime = 0;
    }
    private void ChangedProjectile(BaseAttTrackingProjectile _newProjectile)
    {
        if (_newProjectile == null)
            CurProjectile = OriginProjectile;
        else
            CurProjectile = _newProjectile;
    }
}
