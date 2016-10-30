using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CardController : MonoBehaviour {

	public GameController master;
    public GameObject this_button_;

	public Color open_color_;

	public enum CardStatus {
		CLOSE,
		CHECK,
		OPEN
	}

	public CardStatus status = CardStatus.CLOSE;
	public string image_name_;
	public int id_;
    public Sprite face_;
    public Sprite back_;
    public Sprite blank_;

    public void Reset(string name, int id) {
		// not used?
		status = CardStatus.CLOSE;
		image_name_ = name;
		id_ = id;
	}

	public void SetImage(Sprite sprite) {
		Image button_image;
		button_image = this.GetComponentInChildren<Image> ();
		button_image.sprite = sprite;
	}

	public void FlipFront() {
        if (master.CardsActive == 0)
        {
            master.CardsActive++;
            SetImage(face_);

            status = CardStatus.CHECK;
            this.GetComponentInParent<Button>().interactable = false;

            master.CheckStatus(id_);
        }
    }

	public void FlipBack() {
        SetImage(back_);

		status = CardStatus.CLOSE;
		this.GetComponentInParent<Button> ().interactable = true;
	}

	public void Fix() {
        SetImage(blank_);
        status = CardStatus.OPEN;
		this.GetComponentInParent<Button> ().interactable = false;
//		ColorBlock colors = this.GetComponentInParent<Button> ().colors;
//		colors.disabledColor = open_color_; 
//		this.GetComponentInParent<Button> ().colors = colors;
	}

    public void Delete()
    {
        Destroy(this_button_);
    }
}
