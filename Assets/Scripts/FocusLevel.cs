using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FocusLevel : MonoBehaviour
{
    public float halfXBounds;
    public float halfYBounds;

    public Bounds focusBounds;

    // Update is called once per frame
    void Update()
    {
        Vector3 position = gameObject.transform.position;
        Bounds bounds = new Bounds();
        bounds.Encapsulate(new Vector3(position.x - halfXBounds, position.y - halfYBounds, position.z));
        bounds.Encapsulate(new Vector3(position.x + halfXBounds, position.y + halfYBounds, position.z));
        focusBounds = bounds;
    }
}
