using OrderedActionSequences;
using UnityEngine;
using UnityEngine.VFX;

internal sealed class ToggleVisualEffectAction : MonoBehaviour, IActionSequence
{
    [SerializeField]
    private ParticleSystem _vfx;

    [SerializeField]
    private bool _start = true;

    public ICompletionSource OnEnd()
    {
        return CompletionSource.Completed;
    }

    public ICompletionSource OnStart()
    {
        if (_start)
        {
            _vfx.Play();
        }
        else
        {
            _vfx.Stop();
        }
        return CompletionSource.Completed;
    }
}