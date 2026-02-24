using System.Resources;
using UnityEngine;

public class ScoutCombat : MonoBehaviour
{
    public ResourceManager resourceManager;
    public DrawManager drawManager;
    public scout_movement scoutMovement;
    public ScoutTrail scoutTrail;
    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void DestroyScout()
    {
        {
            //Destroy(scout);
            transform.position = new Vector2(0, 0); //vrat se na zaèátek
            drawManager.ClearAllLines(); //vyèisti všechny cesty
            scoutMovement.Stop(); //zastav pohyb
            scoutTrail.ClearPath(); //vyèisti cestu
            resourceManager.BuyNewScout(); //koupit nového scoutra -  nejde do záporných hodnot, takže pokud nemá dostatek zdrojù, tak se nic nestan.
        }
    }
}
