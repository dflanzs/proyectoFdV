using System;
using UnityEngine;

public class FocusedState : BaseState
{
    private readonly float k_GroundedRadius = 0.2f;
    private bool _focused, _grounded;
    private int _prevDirection;
    private Rigidbody2D rb;
    private bool _flies;
    private Vector2 _scale;
    private float _moveSpeed = 5f; // Velocidad constante hacia el jugador
    private RaycastHit2D hit;
    private Vector2 direction;

    public override void UpdateState(StateManager npc, GameObject player, Transform _groundChecker, Transform _fieldOfView)
    {
        // Limit lateral velocity
        if (rb.velocity.x > npc.getMaxSpeed())
            rb.velocity = new Vector2(rb.velocity.x * 0.99f, rb.velocity.y);

        _focused = false;
        _grounded = false;
        _prevDirection = npc.getDirection();

        Collider2D[] collidersGC = Physics2D.OverlapCircleAll(_groundChecker.position, k_GroundedRadius);
        for(int i = 0; i < collidersGC.Length && !_grounded;  i++){
            if(collidersGC[i].gameObject.CompareTag("Platform")){
                _grounded = true;
            }
        }
        
        if (_flies)
        {
            if (_focused)
            {
                rb.velocity = new Vector2(0, rb.velocity.y);
                //Debug.Log("Shoot");
            }
            else
            {
                npc.setPrevstate(npc.focusedState);
                npc.setDirection(npc.getDirection()); // Cambiar la dirección
                npc.SwitchState(npc.idleState);   
            }
        }
        else if(!_flies)
        {
            hit = Physics2D.Raycast(npc.transform.position, player.transform.position - npc.gameObject.transform.position,
                                    Mathf.Infinity, LayerMask.GetMask("Ground", "Player"));

            if (hit.collider != null)
            {
                if (hit.collider.CompareTag("Platform"))
                {
                    Debug.Log("Ground detected");
                }
                else if (hit.collider.CompareTag("Player"))
                {
                    Debug.Log("Player detected");
                    checkFocus(_fieldOfView);
                }
            }

            if (_focused)
            {
                if (_grounded)
                {
                    if (player.transform.position.x <= npc.transform.position.x)
                    {
                        Debug.Log("Changing irection");
                        _scale = npc.transform.localScale;
                        _scale.x = 1;
                    } else if (player.transform.position.x > npc.transform.position.x){
                        Debug.Log("Changing irection");
                        _scale = npc.transform.localScale;
                        _scale.x = -1;
                    }
                    
                    npc.transform.localScale = _scale; 
                    
                    // Calcular la dirección hacia el jugador
                    direction = player.transform.position - npc.transform.position;
                    if (direction.x > 0)
                        npc.setDirection(1);
                        
                    else if(direction.x < 0)
                        npc.setDirection(-1);

                    rb.velocity = new Vector2(direction.x, rb.velocity.y); // Aplicar velocidad constante hacia el jugador

                }
                else if(!_grounded)
                {
                    rb.velocity = new Vector2(0, rb.velocity.y);

                    npc.setPrevstate(npc.focusedState);
                    npc.setDirection(_prevDirection); // Cambiar la dirección
                    npc.SwitchState(npc.idleState);
                }
            }
            else
            {
                npc.setPrevstate(npc.focusedState);
                npc.setDirection(npc.getDirection()); // Cambiar la dirección
                npc.SwitchState(npc.idleState);
            }
        }
    }

    public override void EnterState(StateManager npc, GameObject player)
    {
        _flies = npc.getFlies();
        _focused = npc.getFocus();
        _prevDirection = npc.getDirection();
        rb = npc.gameObject.GetComponent<Rigidbody2D>();
    }

    public override void OnCollisionEnter(StateManager npc, GameObject player)
    {
        // Mantener la velocidad constante hacia el jugador incluso después de la colisión
        if (player.CompareTag("Player"))
        {
            // Vector2 direction = (player.transform.position - npc.transform.position).normalized;
            rb.velocity = new Vector2(0, rb.velocity.y);
        }
    }

    private void checkFocus(Transform _fieldOfView)
    {
        Collider2D[] collidersFOV = Physics2D.OverlapCircleAll(_fieldOfView.position, _fieldOfView.gameObject.GetComponent<CircleCollider2D>().radius);
    
        for(int i = 0; i < collidersFOV.Length && !_focused; i++){
            if(collidersFOV[i].gameObject.CompareTag("Player")){
                _focused = true;
            }
        }
    }
}