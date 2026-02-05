using System.Collections;
using UnityEngine;

public class ShockWaveEffect : MonoBehaviour
{

    [SerializeField] private float _shockWaveDuration = 0.75f;

    private Coroutine _shockWaveCoroutine;

    private Material _material;

    private static int _waveDistanceFromCenter = Shader.PropertyToID("_WaveDistanceFromCenter");

    private HealthComponent _healthComponent;

    private void Awake()
    {
        _material = GetComponent<Renderer>().material;
        _healthComponent = GetComponentInParent<HealthComponent>();

        _healthComponent.OnDeath += PreventDestruction;
    }

    public void CallShockWave()
    {
        _shockWaveCoroutine = StartCoroutine(ShockWaveAction(0.1f,1f));
    }

    private IEnumerator ShockWaveAction(float startPos, float endPos)
    {

        _material.SetFloat(_waveDistanceFromCenter, startPos);

        float lerpedAmount = 0f;

        float elapsedTime = 0f; 

        while (elapsedTime < _shockWaveDuration)
        {
            elapsedTime += Time.deltaTime;
            lerpedAmount = Mathf.Lerp(startPos, endPos, elapsedTime / _shockWaveDuration);
            _material.SetFloat(_waveDistanceFromCenter, lerpedAmount);
            yield return null;
        }

    }

    private void PreventDestruction(bool obj)
    {
        if (_shockWaveCoroutine != null)
        {
            StopCoroutine(_shockWaveCoroutine);
        }
        gameObject.transform.parent = null;
    }   
}
