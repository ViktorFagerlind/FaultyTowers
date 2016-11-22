using UnityEngine;
using System.Collections;


[RequireComponent (typeof (CircleCollider2D))]

public class Stone : MoveableObject
{
  public enum State
  {
    BeforeLaunch,
    Launching,
    Flying
  }  

  public float m_stretchLimit = 1f;
  public LineRenderer m_bandBack;
  public LineRenderer m_bandFront;

  private State m_state;

  private float m_stretchSquare = 1.0f;
  private SpringJoint2D m_spring;
  private Transform m_slingshot;
  private Ray m_slingshotToStoneRay;
  private Ray m_aroundStoneRay;
  private float m_radius;
  private Vector2 m_velocityX;

  private Vector3 m_initialPosition;


  override protected void Awake()
  {
    base.Awake ();

    m_initialPosition = transform.position;

    m_spring = GetComponent<SpringJoint2D> ();
    m_slingshot = m_spring.connectedBody.transform;
    CircleCollider2D circleColl = GetComponent<CircleCollider2D> ();

    m_stretchSquare = m_stretchLimit * m_stretchLimit;
    m_radius = circleColl.radius;

    m_state = State.BeforeLaunch;
  }

  void Start()
  {
    StringSetup ();

    m_slingshotToStoneRay = new Ray (m_slingshot.position, Vector3.zero);
    m_aroundStoneRay = new Ray (m_bandFront.transform.position, Vector3.zero);
  }

  // ------------------------------------------------------------------------------------------

  void StringSetup()
  {
    m_bandBack.SetPosition (0, m_bandBack.transform.position);
    m_bandFront.SetPosition (0, m_bandFront.transform.position);
    m_bandBack.sortingOrder = 1;
    m_bandFront.sortingOrder = 3;
    m_bandBack.sortingLayerName = "Default";
    m_bandFront.sortingLayerName = "Default";
  }

  // ------------------------------------------------------------------------------------------

  void LateUpdate()
  {
    switch (m_state)
    {
      case State.BeforeLaunch:
        StringUpdate ();
        break;

      case State.Launching:
        if (m_velocityX.sqrMagnitude > m_rigidBody.velocity.sqrMagnitude)
        {
          m_spring.enabled = false;
          m_bandBack.enabled = false;
          m_bandFront.enabled = false;

          m_rigidBody.velocity = m_velocityX;

          m_state = State.Flying;
        }

        m_velocityX = m_rigidBody.velocity;
        
        StringUpdate ();
        break;

      case State.Flying:
        if (m_rigidBody.velocity.sqrMagnitude < 0.001f || (!GetComponent<SpriteRenderer> ().isVisible && transform.position.y < m_initialPosition.y))
        {
          transform.position = m_initialPosition;
          m_bandBack.enabled = true;
          m_bandFront.enabled = true;
          m_rigidBody.isKinematic = true;

          m_state = State.BeforeLaunch;
        }
        break;
    }

  }

  // ------------------------------------------------------------------------------------------

  void StringUpdate()
  {
    Vector2 bandToStone = transform.position - m_bandFront.transform.position;
    m_aroundStoneRay.direction = bandToStone;

    Vector3 bandHoldPosition = m_aroundStoneRay.GetPoint (bandToStone.magnitude + m_radius);
    m_bandBack.SetPosition (1, bandHoldPosition);
    m_bandFront.SetPosition (1, bandHoldPosition);
  }

  // ------------------------------------------------------------------------------------------

  override protected void OnGrabbed ()
  {
    if (m_state != State.BeforeLaunch)
      return;
  }

  // ------------------------------------------------------------------------------------------

  override protected void OnDropped ()
  {
    if (m_state != State.BeforeLaunch)
      return;
    
    m_spring.enabled = true;
    m_rigidBody.isKinematic = false;
    m_velocityX = m_rigidBody.velocity;

    m_state = State.Launching;
  }

  // ------------------------------------------------------------------------------------------

  override protected void OnMoved ()
  {
    if (m_state != State.BeforeLaunch)
      return;

    Vector2 slingshotToObjectDirection = transform.position - m_slingshot.position;
    Vector3 newPosition = new Vector3 (transform.position.x, transform.position.y, transform.position.z);

    if (slingshotToObjectDirection.sqrMagnitude > m_stretchSquare)
    {
      m_slingshotToStoneRay.direction = slingshotToObjectDirection;
      newPosition = m_slingshotToStoneRay.GetPoint (m_stretchLimit);
    }

    transform.position = newPosition;
  }

  // ------------------------------------------------------------------------------------------

}
