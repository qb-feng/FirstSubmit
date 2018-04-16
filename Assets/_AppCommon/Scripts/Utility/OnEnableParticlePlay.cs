using UnityEngine;

public class OnEnableParticlePlay : MonoBehaviour
{
    public void OnEnable()
    {
        GetComponent<ParticleSystem>().Play(true);
    }
}
