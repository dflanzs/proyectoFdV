using System.IO.Pipes;
using UnityEngine;

public class AttackState : BaseState
{
    private bool _flies;
    private float _timer;
    private RaycastHit2D focusRC;

    public override void EnterState(StateManager npc, GameObject player)
    {
        _flies = npc.getFlies();
        _timer = npc.getShootCooldown(); // Instant shooting
        npc.setFocus(true);
    }

    public override void UpdateState(StateManager npc, GameObject player, Transform _groundChecker, Transform _fieldOfView)
    {
        if (!npc.SpriteAnimator.IsPlaying("idleAnimation"))
            npc.idle();
        
        if (_flies)
        {
            focusRC = Physics2D.Raycast(npc.transform.position, npc.getTarget(player, npc), Mathf.Infinity, LayerMask.GetMask("Ground", "Player"));

            if (focusRC.collider != null && (focusRC.collider.CompareTag("Player") || focusRC.collider.CompareTag("npcCollision")))
                npc.setFocus(npc.checkFocus(_fieldOfView));

            if (npc.getFocus())
            {
                _timer += Time.deltaTime;

                if (_timer >= npc.getShootCooldown())
                {
                    npc.ShootBullet(npc, player);
                    _timer = 0f;
                }
            }
            else
            {
                npc.setPrevstate(npc.attackState);
                npc.SwitchState(npc.idleState);  
            }
        }
        else
        {
            if (npc.checkPlayerCollision())
            {
                if (_timer < npc.getShootCooldown())
                    _timer += Time.deltaTime;
                else
                {
                    npc.attack();
                    _timer = 0;
                }
            }
            else
            {
                npc.setFocus(false);
                npc.setPrevstate(npc.attackState);
                npc.SwitchState(npc.idleState);  
            }
        }

        npc.setFocus(false);
    }

    private void OnDrawGizmosSelected(StateManager npc) 
    {
        Gizmos.DrawWireSphere(npc.getGun().transform.position, npc.getShootRange());    
    }

    public override void OnCollisionEnter(StateManager npc, GameObject player) { }
}
