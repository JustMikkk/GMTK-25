using System.Collections.Generic;
using UnityEngine;

public class CubesHolder : MonoBehaviour
{
    public List<CubeBasic> cubes;

    public void ResetCubesPositions() {
        foreach (CubeBasic cube in cubes) {
            cube.transform.localPosition = new Vector3(cube.transform.localPosition.x, 0, cube.transform.localPosition.z);
        }
    }
}
