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

    private bool is_moving;
    public Vector3 target;
    private Vector3 speed;
    public void Move()
    {
        is_moving = true;
        Vector3 current_position = this_button_.transform.localPosition;
        speed = (target - current_position) / 2.0f; // 2s to go there.
    }

    void Update()
    {
        if (is_moving)
        {
            Vector3 current_position = this_button_.transform.localPosition;
            float dist = Vector3.Distance(current_position, target);
            if (dist < speed.magnitude * Time.deltaTime)
            {
                this_button_.transform.localPosition = target;
                is_moving = false;
                master.show_success_menu_button.SetActive(true);
            }
            else
            {
                Vector3 new_position = current_position + speed * Time.deltaTime;
                this_button_.transform.localPosition = new_position;
            }
        }
    }
}
