using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerDeathState : BaseState
{
    private const float WAITINGTIMETORELOADLVL = 3f;

    private int _sceneIndex = -1;

    public PlayerDeathState(BasePlayer player) : base(player)
    {
        _sceneIndex = SceneManager.GetActiveScene().buildIndex;
    }

    public override void Enter()
    {
        Debug.Log($"End...");

        _player.Clear();

        _player.StartCoroutine(TimerRoutine());
    }

    private IEnumerator TimerRoutine()
    {
        yield return new WaitForSeconds(WAITINGTIMETORELOADLVL);

        Debug.Log($"Chao...");

        SceneManager.LoadScene(_sceneIndex);
    }

    public override void Exit() 
    {
        
    }
}
