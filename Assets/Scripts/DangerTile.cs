using UnityEngine;

public class DangerTile : MonoBehaviour
{
    public ResourceManager resourceManager;
    public DrawManager drawManager;


    public void DestroyScout(GameObject scout) 
    {
        {
            Rigidbody2D rb = scout.GetComponent<Rigidbody2D>();
            ScoutTrail scoutTrail = scout.GetComponent<ScoutTrail>();
            scout_movement scoutMovement = scout.GetComponent<scout_movement>();

            //Destroy(scout);
            scout.transform.position = new Vector2(0, 0); //vrat se na zaèátek
            drawManager.ClearAllLines(); //vyèisti všechny cesty
            scoutMovement.Stop(); //zastav pohyb
            scoutTrail.ClearPath(); //vyèisti cestu
            resourceManager.BuyNewScout(); //koupit nového scoutra -  nejde do záporných hodnot, takže pokud nemá dostatek zdrojù, tak se nic nestan.
        }
    }
}
