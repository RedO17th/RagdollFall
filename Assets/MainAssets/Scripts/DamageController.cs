using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageController : BasePlayerController
{
    [SerializeField] private BaseLimb[] _limbs;

    private CameraController _cameraController = null;

    private BaseLimb _testLimb = null;

    public override void Initialize(BasePlayer player)
    {
        _cameraController = player.GetController<CameraController>();

        _testLimb = _limbs[0];
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            TestCollision();
        }
    }

    private void TestCollision()
    {
        StartCoroutine(Routine());
    }

    private IEnumerator Routine()
    {
        Debug.Log($"DamageController.Damege: Lomb price { _testLimb.Price }");

        _cameraController.ShiftLookAt(_testLimb.transform);

        //Time.timeScale = 0.1f;

        yield return new WaitForSecondsRealtime(8f);

        //Time.timeScale = 1f;

        //_cameraController.SomeMeth();
    }

    public override void Clear()
    {
        
    }
}
