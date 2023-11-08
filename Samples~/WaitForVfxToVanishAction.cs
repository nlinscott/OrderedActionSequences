using OrderedActionSequences;
using System.Collections;
using UnityEngine;

internal sealed class WaitForVfxToVanishAction : MonoBehaviour, IActionSequence
{
    [SerializeField]
    private ParticleSystem _vfx;

    public ICompletionSource OnEnd()
    {
        _vfx.Stop();

        CompletionSource src = new CompletionSource();

        StartCoroutine(WaitForVfx(src));

        return src;
    }

    public ICompletionSource OnStart()
    {
        _vfx.Play();
        return CompletionSource.Completed;
    }

    private IEnumerator WaitForVfx(ICompletionToken token)
    {
        yield return new WaitUntil(() =>
        {
            return _vfx.particleCount <= 0;
        });

        token.MarkCompleted();
    }
}