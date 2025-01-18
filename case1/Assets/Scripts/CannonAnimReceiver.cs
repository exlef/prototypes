using UnityEngine;

public class CannonAnimReceiver : MonoBehaviour
{
    // this is being called from the animation event for cannon shoot
    public void ShootEvent()
    {
        GameManager.instance.SpawnNormieMobOnCannonFire(); 
    }
}
