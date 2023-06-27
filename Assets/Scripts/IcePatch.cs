using UnityEngine;

public class IcePatch : MonoBehaviour
{
    public float speedModifier = -0.8f;
    public bool isTankingPatch = false;
    
    void OnTriggerEnter2D(Collider2D other)
    {
        var raidMember = other.attachedRigidbody.gameObject.GetComponent<PlayerCharacter>();
        if (raidMember is not null)
        {
            raidMember.speedCoefficient += speedModifier;
            raidMember.isVulnerable = false;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        var raidMember = other.attachedRigidbody.gameObject.GetComponent<PlayerCharacter>();
        if (raidMember is not null)
        {
            raidMember.speedCoefficient -= speedModifier;
            raidMember.isVulnerable = true;
        }
    }
}
