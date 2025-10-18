using UnityEngine.Pool;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DamageManager : MonoBehaviour
{
    public GameObject damageLabelPrefab; // Assign your damage label prefab here

    public void ShowDamage(Vector3 position, float damage, Transform parent)
    {
        // Instantiate the damage label at the position
        GameObject damageLabel = Instantiate(damageLabelPrefab, position, Quaternion.identity, parent);

        // Get the DamageLabel component and set the damage amount
        DamageLabel label = damageLabel.GetComponent<DamageLabel>();
        label.SetDamage(damage);
    }
}

