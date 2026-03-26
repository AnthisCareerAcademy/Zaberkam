using UnityEngine;
using UnityEngine.UI;
public class CharacterSelect : MonoBehaviour
{
    public Button myButton;
    public Button mageSelect;
    public Button warriorSelect;
    public GameObject PlayerMage;
    public GameObject PlayerWarrior;
    public GameObject CharacterParent;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void OnButtonPress()
    {
        myButton.gameObject.SetActive(false);
        CharacterParent.SetActive(true);
    }
    public void OnMageSelect()
    {
        Instantiate(PlayerMage);
        CharacterParent.SetActive(false);
    }
    public void OnWarriorSelect()
    {
        Instantiate(PlayerWarrior);
        CharacterParent.SetActive(false);
    }

}
