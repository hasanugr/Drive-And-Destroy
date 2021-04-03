using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class PostProcessControl : MonoBehaviour
{
    [SerializeField]
    private PostProcessVolume _postProcessVolume;

    public void ActivateDOF()
    {
        if (_postProcessVolume != null)
        {
            DepthOfField depthOfField;
            if (_postProcessVolume.profile.TryGetSettings(out depthOfField))
            {
                depthOfField.active = true;
                LeanTween.value(gameObject, 10f, 0.1f, 0.5f).setOnUpdate((float val) =>
                {
                    depthOfField.focusDistance.value = val;
                });
            }
        }
    }
    public void DeactivateDOF()
    {
        if (_postProcessVolume != null)
        {
            DepthOfField depthOfField;
            if (_postProcessVolume.profile.TryGetSettings(out depthOfField))
            {
                LeanTween.value(gameObject, 0.1f, 10f, 0.5f).setOnUpdate((float val) =>
                {
                    depthOfField.focusDistance.value = val;
                    if (val >= 10f)
                    {
                        depthOfField.active = false;
                    }
                });
            }
        }
    }
}
