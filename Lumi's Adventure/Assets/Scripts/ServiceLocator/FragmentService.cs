using UnityEngine;
using System.Collections.Generic;

public class FragmentService : MonoBehaviour, IFragmentService
{
    private List<Transform> fragments = new List<Transform>();

    private void Awake()
    {
        ServiceLocator.Register<IFragmentService>(this);
    }

    public void RegisterFragment(Transform fragment)
    {
        if (!fragments.Contains(fragment))
            fragments.Add(fragment);
    }

    public void UnregisterFragment(Transform fragment)
    {
        fragments.Remove(fragment);
    }

    public Transform GetNearestFragment(Vector3 fromPosition)
    {
        Transform nearest = null;
        float minDist = float.MaxValue;

        foreach (Transform frag in fragments)
        {
            if (!frag) continue;

            float dist = Vector3.Distance(fromPosition, frag.position);
            if (dist < minDist)
            {
                minDist = dist;
                nearest = frag;
            }
        }

        return nearest;
    }
}
