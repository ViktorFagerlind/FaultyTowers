using UnityEngine;
using System.Collections;


[RequireComponent (typeof (CircleCollider2D))]

public class Stone : MoveableObject
{

  public float m_stretchLimit = 1f;
  public LineRenderer m_bandBack;
  public LineRenderer m_bandFront;

  private float m_stretchSquare = 1.0f;
  private SpringJoint2D m_spring;
  private Transform m_slingshot;
  private Ray m_slingshotToStoneRay;
  private Ray m_aroundStoneRay;
  private float m_radius;
  private Vector2 m_velocityX;

  override protected void Awake()
  {
    base.Awake ();

    m_stretchSquare = m_stretchLimit * m_stretchLimit;
    m_spring = GetComponent<SpringJoint2D> ();
    m_slingshot = m_spring.connectedBody.transform;
  }

  void Start()
  {
    StringSetup ();

    m_slingshotToStoneRay = new Ray (m_slingshot.position, Vector3.zero);
    m_aroundStoneRay = new Ray (m_bandFront.transform.position, Vector3.zero);

    CircleCollider2D circleColl = GetComponent<CircleCollider2D> ();
    m_radius = circleColl.radius;
  }

  // ------------------------------------------------------------------------------------------

  void LateUpdate()
  {
    StringUpdate ();

    if (m_spring != null)
    {
      if (!GetComponent<Rigidbody2D>().isKinematic && m_velocityX.sqrMagnitude > GetComponent<Rigidbody2D>().velocity.sqrMagnitude)
      {
        Destroy (m_spring);
        GetComponent<Rigidbody2D>().velocity = m_velocityX;
      }

      if (!m_ongoingMove)
        m_velocityX = GetComponent<Rigidbody2D>().velocity;

      StringUpdate ();

    }
    else
    {
      m_bandBack.enabled = false;
      m_bandFront.enabled = false;
    }
  }

  void StringSetup()
  {
    m_bandBack.SetPosition (0, m_bandBack.transform.position);
    m_bandFront.SetPosition (0, m_bandFront.transform.position);
    m_bandBack.sortingOrder = 1;
    m_bandFront.sortingOrder = 3;
    m_bandBack.sortingLayerName = "Default";
    m_bandFront.sortingLayerName = "Default";
  }

  override protected void OnGrabbed ()
  {
    m_spring.enabled = false;
  }

  override protected void OnDropped ()
  {
    m_spring.enabled = true;
    GetComponent<Rigidbody2D>().isKinematic = false;
  }

  override protected void OnMoved ()
  {
    Vector2 slingshotToObjectDirection = transform.position - m_slingshot.position;
    Vector3 newPosition = new Vector3 (transform.position.x, transform.position.y, transform.position.z);

    if (slingshotToObjectDirection.sqrMagnitude > m_stretchSquare)
    {
      m_slingshotToStoneRay.direction = slingshotToObjectDirection;
      newPosition = m_slingshotToStoneRay.GetPoint (m_stretchLimit);
    }

    transform.position = newPosition;
  }

  void StringUpdate()
  {
    Vector2 bandToStone = transform.position - m_bandFront.transform.position;
    m_aroundStoneRay.direction = bandToStone;

    Vector3 hold = m_aroundStoneRay.GetPoint (bandToStone.magnitude + m_radius);
    m_bandBack.SetPosition (1, hold);
    m_bandFront.SetPosition (1, hold);
  }
}
