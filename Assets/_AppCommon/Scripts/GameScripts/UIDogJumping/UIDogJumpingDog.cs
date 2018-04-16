using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Collections;
using DG.Tweening;

enum DogJumpDirect
{
    Left,
    Up,
    Right
}
public class UIDogJumpingDog : UIBaseInit
{

    private bool isJump = false;
    private Rigidbody2D myRighdbody;
    private Image CatImage;
    private bool isRight = false;
    private DogJumpDirect NowClick;
    private Image Rainbow { get { return Get<Image>("Rainbow"); } }

    void Awake()
    {
        myRighdbody = GetComponent<Rigidbody2D>();
        CatImage = GetComponent<Image>();
        Rainbow.enabled = false;
    }

    void Update()
    {
        if (!UIDogJumping.Instance.audio_in)
        {
            if (!isJump)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    isJump = true;
                    StartCoroutine(CatJump());
                }
            }
        }
    }

    IEnumerator CatJump()
    {
        UIDogJumping.Instance.InitSpringboard();
        Vector3 select = UIManager.Instance.WorldCamera.ScreenToWorldPoint(Input.mousePosition);
        Vector2 target = select - transform.position;
        target = target.normalized;
        if (target.normalized.x > -0.3&& target.normalized.x < 0.3)
        {
            NowClick = DogJumpDirect.Up;
        }
        else if (target.normalized.x < -0.3)
        {
            NowClick = DogJumpDirect.Left;
        }
        else if (target.normalized.x > 0.3)
        {
            NowClick = DogJumpDirect.Right;
            ;
        }
        switch (NowClick)
        {
            case DogJumpDirect.Left:
                myRighdbody.velocity = Vector2.zero;
                myRighdbody.AddForce(Vector2.up * 325);
                myRighdbody.AddForce(Vector2.left * 100);
                break;
            case DogJumpDirect.Up:
                myRighdbody.velocity = Vector2.zero;
                myRighdbody.AddForce(Vector2.up * 325);
                break;
            case DogJumpDirect.Right:
                myRighdbody.velocity = Vector2.zero;
                myRighdbody.AddForce(Vector2.up * 325);
                myRighdbody.AddForce(Vector2.right * 100);
                break;
        }

        //myRighdbody.AddForce(Vector2.up * 325);
        //myRighdbody.AddForce(target * 400);
        yield return new WaitForSeconds(1.2f);
        isJump = false;
    }

    void OnCollisionStay2D(Collision2D e)
    {
        if (e.transform.name == "BucketDown")
        {
            return;
        }
        //float tempX = e.relativeVelocity.x;
        if (e.transform.GetComponent<Rigidbody2D>() != null)
        {
            Vector2 tempX = e.transform.GetComponent<Rigidbody2D>().velocity;
            //myRighdbody.velocity = new Vector2(tempX, 0);
            myRighdbody.velocity = tempX;
        }
    }

    void OnBecameInvisible()
    {
        if (!isRight)
        {
            if (UIDogJumping.Instance.OnRefreshCat != null)
                UIDogJumping.Instance.OnRefreshCat();
        }
    }

    void OnTriggerEnter2D(Collider2D e)
    {
        if (e.GetComponent<UIDogJumpingFish>() != null)
        {
            if (UIDogJumping.Instance.Current_Answer == e.GetComponent<UIDogJumpingFish>().currentText)
            {
                e.GetComponent<UIDogJumpingFish>().FishImage.sprite = e.GetComponent<UIDogJumpingFish>().GetS("UIDogJumping_BoneEated");
                myRighdbody.velocity = Vector2.zero;
                isJump = true;
                StartCoroutine(AnswerRight());
            }
            else
            {                
                myRighdbody.velocity = Vector2.zero;
                isJump = true;
                StartCoroutine(AnswerWrong());
            }

        }
    }
    IEnumerator AnswerRight()
    {

        myRighdbody.simulated = false;
        CatImage.sprite = GetS("UIDogJumping_Risus");
        yield return new WaitForSeconds(0.5f);
        Rainbow.enabled = true;
        CatImage.sprite = GetS("UIDogJumping_Smile");
        yield return new WaitForSeconds(0.3f);
        this.transform.DOMoveY(this.transform.position.y + 5, 1);
        isRight = true;
        UIDogJumping.Instance.DecideStar(true);
    }

    IEnumerator AnswerWrong()
    {
        myRighdbody.simulated = false;
        CatImage.sprite = GetS("UIDogJumping_Cry");
        CatImage.DOFade(0, 0.3f).SetLoops(3).From();
        UIDogJumping.Instance.DecideStar(false);
        yield return new WaitForSeconds(1);
        this.transform.GetComponent<MeshRenderer>().enabled = false;
        Destroy(this.gameObject);
        // UICatJumping.Instance.CreateCat();


    }



}


