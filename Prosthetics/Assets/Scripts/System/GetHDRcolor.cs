using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Gets a HDR color given a normal color and an intensity
/// </summary>
public class GetHDRcolor : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    /// <summary>
    /// Calculate the HDR color
    /// </summary>
    /// <param name="color"></param>
    /// <param name="intensity"></param>
    /// <returns></returns>
    public static Vector4 GetColorInHDR(Color color, float intensity)
    {
        // if not using gamma color space, convert from linear to gamma
#if UNITY_COLORSPACE_GAMMA
        color = LinearToGammaSpace(color);
#endif
        // apply intensity exposure
        color *= Mathf.Pow(2.0f, intensity) * 0.7490196f;
        // if not using gamma color space, convert back to linear
# if UNITY_COLORSPACE_GAMMA
        color = GammaToLinearSpace(color);
#endif

        return color;
    }
}
