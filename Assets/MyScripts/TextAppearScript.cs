using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Script created by Melvin Jr Sanggalan
 * Last updated 22/05/2023
 * Script allows text to be displayed to the player's screen and then disappears after a while.
 */

public class TextAppearScript : MonoBehaviour
{
    //reference to the text
    public GameObject theText;
    public int visibleTime;


    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(textAppearAndDisappear(theText));
    }

    IEnumerator textAppearAndDisappear(GameObject textBeingUsed)
    {
        theText.SetActive(true);

        //visible for 10 seconds
        yield return new WaitForSeconds(visibleTime);

        theText.SetActive(false);


    }

}
