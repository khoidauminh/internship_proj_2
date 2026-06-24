using UnityEngine;

public class PlayerHurtBoxController : MonoBehaviour
{
    private int enemyLayer;

    private void Start()
    {
        enemyLayer = LayerMask.NameToLayer("Enemy");
    }

    private void OnTriggerStay(Collider other)
    {
        ColliderController controller = transform.parent.GetComponent<ColliderController>();

        if (other.gameObject.layer == enemyLayer && controller.HurtTimer <= 0)
        {
            controller.Hurt();
            Debug.Log("Hurt!!!!!!!!");
        }
    }
}
