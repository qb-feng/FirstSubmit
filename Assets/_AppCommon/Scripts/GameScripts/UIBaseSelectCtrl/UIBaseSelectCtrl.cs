using UnityEngine;
using DG.Tweening;

public abstract class UIBaseSelectCtrl : UIBaseInit
{
    protected bool m_isPointDown = false;
    private Vector2 m_currentTouchPosition = Vector2.zero;
    private int m_currentPointerId = 0;

    protected virtual Vector3 RightPosition { get { return m_collider==null?Vector3.zero:m_collider.transform.position; } }
    protected virtual bool PointUpSpeedZero { get { return true; } }

    protected bool m_isKinematics = true;

    protected Vector3 m_originalPosition;
    private Collider2D m_collider;
    protected Collider2D m_cacheCollider;
    protected Rigidbody2D m_rigidbody2d;
    protected Tweener m_rigidbody2dTween;
    // Use this for initialization
    protected virtual void Start()
    {
        m_rigidbody2d = GetComponent<Rigidbody2D>();
        m_originalPosition = transform.position;
        var originalPosition = transform.position;
        UGUIEventListener.Get(gameObject).onPointerDown = (d) =>
        {
            if (m_isKinematics)
                m_rigidbody2d.isKinematic = true;
            m_currentPointerId = d.pointerId;
            originalPosition = transform.position;
        };
        UGUIEventListener.Get(gameObject).onPointerUp = d =>
        {
            if (m_isKinematics)
                m_rigidbody2d.isKinematic = false;
            GoPointUp(d);
        };
        UGUIEventListener.Get(gameObject).onBeginDrag = (d) => m_isPointDown = true;
        UGUIEventListener.Get(gameObject).onEndDrag = (d) => m_isPointDown = false;
        UGUIEventListener.Get(gameObject).onDrag = d =>
        {
            if (m_isKinematics)
            {
                var currentWorld = d.pressEventCamera.ScreenToWorldPoint(d.position);
                var originalWorld = d.pressEventCamera.ScreenToWorldPoint(d.pressPosition);
                transform.position = (Vector2)(originalPosition + currentWorld - originalWorld);
            }
        };
    }

    void Update()
    {
        if (!m_isKinematics)
        {
            m_currentTouchPosition = Input.mousePosition;
            if (Input.touchCount > 0)
            {
                m_currentTouchPosition = Input.GetTouch(m_currentPointerId).position;
            }
        }
    }

    void FixedUpdate()
    {
        if (!m_isKinematics)
        {
            if (m_isPointDown)
            {
                Vector2 target = UIManager.Instance.WorldCamera.ScreenToWorldPoint(m_currentTouchPosition);
                Vector2 velocity = m_rigidbody2d.velocity;
                Vector2.SmoothDamp(m_rigidbody2d.position, target, ref velocity, 0.1f, 1000, Time.deltaTime);
                m_rigidbody2d.velocity = velocity;
            }
        }
    }

    protected virtual void GoPointUp(UnityEngine.EventSystems.PointerEventData arg0)
    {
        if (PointUpSpeedZero)
            m_rigidbody2d.velocity = Vector3.zero;
        if (m_collider && GetMatch())
        {
            RightRigibodyTween();
        }
        else
        {
            WrongRigibodyTween();
            if (m_collider)
            {
                WrongMatchAction();
            }
        }
        m_rigidbody2dTween.OnPlay(() =>
        {
            m_collider = null;
            GetComponent<Collider2D>().enabled = false;
        });
    }

    protected virtual void RightRigibodyTween()
    {
        m_rigidbody2dTween = m_rigidbody2d.DOMove(RightPosition, 0.2f);
        m_rigidbody2dTween.OnComplete(() =>
        {
            GetComponent<Collider2D>().enabled = true;
            RighMatchAction();
        });
    }

    protected virtual void WrongRigibodyTween()
    {
        m_rigidbody2dTween = m_rigidbody2d.DOMove(m_originalPosition, 0.4f);
        Debug.Log("******Move Tween********");
        m_rigidbody2dTween.OnComplete(() =>
        {
            GetComponent<Collider2D>().enabled = true;
        });
    }

    protected abstract void RighMatchAction();

    protected abstract void WrongMatchAction();

    protected abstract bool GetMatch();

    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        m_collider = other;
        m_cacheCollider = m_collider;
    }

    protected virtual void OnTriggerExit2D(Collider2D other)
    {
        if (m_cacheCollider == other)
            m_collider = null;
    }
}
