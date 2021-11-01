using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public ParticleSystem moveCusorPrefab;
    public ParticleSystem attackCursorPrefab;
    private MovementController mc;
    Unit unit;
    private void Awake()
    {
        moveCusorPrefab = Instantiate(moveCusorPrefab);
        attackCursorPrefab = Instantiate(attackCursorPrefab);
        UIController.onUpdateUI += SetPlayerController;
    }
    private void SetPlayerController(Unit _unit)
    {
        if (mc != null)
        {
            mc.OnControllable -= Mc_OnControllable;
            mc.OnUncontrollable -= Mc_OnUncontrollable;
        }

        unit = _unit;
        mc = unit.GetComponent<MovementController>();
        mc.OnControllable+= Mc_OnControllable;
        mc.OnUncontrollable+= Mc_OnUncontrollable;
    }
    private void LateUpdate()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Input.GetMouseButtonDown(1) && CursorControler.instance.cursorSelectMode == CursorControler.CursorSelectMode.Normal)
        {
            if (Physics.Raycast(ray, out hit, 50, LayerMask.GetMask("Ground") | LayerMask.GetMask("Unit")))
            {
                Unit TarUnit = hit.transform.GetComponent<Unit>();
                if (TarUnit != null)
                {
                    if (TarUnit.team != unit.team)//敵人
                    {
                        mc.ChangeState(MovementController.ComandState.AttackSomone, Vector3.zero, TarUnit);
                    }
                }
                else
                {
                    //move
                    mc.ChangeState(MovementController.ComandState.Move, hit.point, null);
                    moveCusorPrefab.transform.position = new Vector3(hit.point.x, 0.1f, hit.point.z);
                    moveCusorPrefab.Simulate(0, true, true);
                    moveCusorPrefab.Play();
                }
            }
        }
        else if (Input.GetKeyDown(KeyCode.A))
        {
            CursorControler.instance.ChangeMode(CursorControler.CursorSelectMode.Attack);
        }
        else if (CursorControler.instance.cursorSelectMode== CursorControler.CursorSelectMode.Attack && Input.GetMouseButtonDown(0))
        {
            if (Physics.Raycast(ray, out hit, 50, LayerMask.GetMask("Ground") | LayerMask.GetMask("Unit")))
            {
                Unit TarUnit = hit.transform.GetComponent<Unit>();
                if (TarUnit != null)//點人
                {
                        if (TarUnit.team != unit.team)//敵人
                        {
                            mc.ChangeState(MovementController.ComandState.AttackSomone, Vector3.zero, TarUnit);
                        }
                        CursorControler.instance.ChangeMode(CursorControler.CursorSelectMode.Normal);
                }
                else//點地
                {
                    //move
                    mc.ChangeState(MovementController.ComandState.Attack, hit.point, null);
                    attackCursorPrefab.transform.position = new Vector3(hit.point.x, 0.1f, hit.point.z);
                    attackCursorPrefab.Simulate(0, true, true);
                    attackCursorPrefab.Play();
                    CursorControler.instance.ChangeMode(CursorControler.CursorSelectMode.Normal);
                }
            }
        }


    }


    void Mc_OnControllable()
    {
        CursorControler.instance.RecoverUsed();
    }

    void Mc_OnUncontrollable()
    {
        CursorControler.instance.ChangeMode(CursorControler.CursorSelectMode.Useless);

    }

}
