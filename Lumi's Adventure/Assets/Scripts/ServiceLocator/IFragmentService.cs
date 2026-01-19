using UnityEngine;

public interface IFragmentService
{
    Transform GetNearestFragment(Vector3 fromPosition);
    void RegisterFragment(Transform fragment);
    void UnregisterFragment(Transform fragment);
}
