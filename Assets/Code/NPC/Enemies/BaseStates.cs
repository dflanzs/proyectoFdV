using UnityEngine;

public abstract class BaseState
{
    public abstract void EnterState(StateManager npc, GameObject player, int intAux, bool boolAux);

    public abstract void UpdateState(StateManager npc, GameObject player, Transform _groundChecker);

    public abstract void OnCollisionEnter(StateManager npc, GameObject player);
}
