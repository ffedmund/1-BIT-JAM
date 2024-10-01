using UnityEngine;

public class InteractNotice : MonoBehaviour {
    public string interactText;

    private void OnTriggerEnter2D(Collider2D other) {
        if(other.CompareTag("Player"))
        {
            FindAnyObjectByType<HUDController>().SetInteractText(interactText);
        }
    }


    private void OnTriggerExit2D(Collider2D other) {
        if(other.CompareTag("Player"))
        {
            HUDController controller = FindAnyObjectByType<HUDController>();
            if(controller.interactText?.GetParsedText().Equals(interactText) ?? false)
                controller.SetInteractText("");
        }
    }
}