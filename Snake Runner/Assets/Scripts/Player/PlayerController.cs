using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerController : MonoBehaviour
{
	private Animator _animator;
	private Collider _collider;
	private AudioSource _audioSource;
	[SerializeField] private AudioClip bite_sfx;
	[SerializeField] private AudioClip death_sfx;
	[SerializeField] private GameObject speedFX;
	private Camera _camera;
	private float _cameraOffset;

	[SerializeField] private Renderer _renderer;
	public Color color { get; private set; }
	[SerializeField] private Color defaultColor = Color.green;
	[SerializeField] private float _colorChangeTime = 1.0f;
	private bool isBodyReady = false;
	private bool isChangingColor = false;
	private bool isFeverMode = false;
	private bool isMovingToCenter = false;

	[SerializeField] float forwardSpeed = 20;
	[SerializeField] float sideSpeed = 10;

	[SerializeField] float distanceBeetween = 0.2f;
	[SerializeField] List<GameObject> bodyParts = new List<GameObject>();
	List<GameObject> snakeBody = new List<GameObject>();

	private bool isInputHeld = false;
	private float horizontalInput = 0f;
	private Vector3 touchPosition = Vector3.zero;

	[SerializeField] float turnSmoothTime = 0.1f;
	float turnSmoothVelocity;

	float countUp = 0;

	[SerializeField] private int gemsWasEted = 0;

	public event Action deathEvent;
	public event Action eatHumanEvent;
	public event Action eatGemEvent;

	public Transform eatPoint;
	public static PlayerController Instance = null;

	public float swallowSpeed = 5f;
	public float swallowDuration = 0.5f;

	void Start()
	{
		if (Instance == null)
		{
			Instance = this;
		}
		else if (Instance != this)
		{
			Destroy(gameObject);
		}

		_animator = GetComponent<Animator>();
		_collider = GetComponent<Collider>();
		_audioSource = GetComponent<AudioSource>();
		_camera = Camera.main;
		_cameraOffset = _camera.transform.position.z - transform.position.z;
		color = defaultColor;

		CreateBodyParts();
	}

	void Update()
    {
		GetInput();
		CameraFollow();
	}

    private void FixedUpdate()
    {
		Movement();

		if (bodyParts.Count > 0)
		{
			CreateBodyParts();
		}
		SideRotation();
		SnakeMovement();
		transform.position = new Vector3 (Mathf.Clamp(transform.position.x, -6, 6), transform.position.y, transform.position.z);
	}

	private void OnTriggerEnter(Collider collider)
	{
		ICollectable collectable = collider.GetComponent<ICollectable>();
		if (collectable != null)
		{
			collectable.Collect(isFeverMode);
		}

		DamageObstacle damaging = collider.GetComponent<DamageObstacle>();
		if (damaging != null)
		{
			if (!isFeverMode)
			{
				damaging.Damage();
			}
			else
			{
				damaging.Collect(isFeverMode);
			}
		}
	}

	public void Death()
    {
		if (deathEvent != null && !isFeverMode)
		{
			_audioSource.PlayOneShot(death_sfx);
			deathEvent();
		}
	}

	public void Eat(string target)
    {
		_animator.SetTrigger("Eat");
		switch (target)
		{
			case "Human":
				eatHumanEvent?.Invoke();
				break;
			case "Gem":
				eatGemEvent?.Invoke();
				++gemsWasEted;
                if (gemsWasEted >= 3)
                {
					gemsWasEted = 0;
					if (!isFeverMode)
					{
						StartCoroutine(FeverMode());
					}
				}
				break;
			default:
				eatHumanEvent?.Invoke();
				break;
		}
	}

	public void SetColor(Color newColor)
    {
        if (!isChangingColor)
        {
			StartCoroutine(ColorLerp(newColor, _colorChangeTime));
			color = newColor;
		}
	}

	IEnumerator ColorLerp(Color newColor, float time)
    {
		Color actualColor = _renderer.material.color;
		isChangingColor = true;
		float elapsedTime = 0.0f;
		float totalTime = time;
		while (elapsedTime < totalTime)
		{
			elapsedTime += Time.deltaTime;
			_renderer.material.color = Color.Lerp(actualColor, newColor, elapsedTime / totalTime);
			yield return null;
		}
		isChangingColor = false;
	}

	public void PlaySound(AudioClip sfx)
    {
		_audioSource.PlayOneShot(sfx);
	}

	IEnumerator FeverMode()
    {
		isFeverMode = true;
		StartCoroutine(MoveToCenter(1f));
		float elapsedTime = 0.0f;
		float totalTime = 5f;
		forwardSpeed *= 3f;
		speedFX.SetActive(true);
		while (elapsedTime < totalTime)
		{
			_camera.fieldOfView = Mathf.Lerp(60, 80, elapsedTime / totalTime);
			elapsedTime += Time.deltaTime;
			yield return null;
		}
		speedFX.SetActive(false);
		GameManager.Instance.ResetGemPoints();
		gemsWasEted = 0;
		forwardSpeed /= 3f;
		elapsedTime = 0.0f;
		totalTime = 0.5f;
		while (elapsedTime < totalTime)
		{
			_camera.fieldOfView = Mathf.Lerp(80, 60, elapsedTime / totalTime);
			elapsedTime += Time.deltaTime;
			yield return null;
		}
		isFeverMode = false;
	}

	private IEnumerator MoveToCenter(float time)
	{
		isMovingToCenter = true;
		float elapsedTime = 0;
		float _duration = time;
		float startX = transform.position.x;
		while (elapsedTime < _duration)
		{
			float posX = Mathf.Lerp(startX, 0, elapsedTime / _duration);
			transform.position = new Vector3(posX, transform.position.y, transform.position.z);
			elapsedTime += Time.deltaTime;
			yield return null;
		}
		isMovingToCenter = false;
	}

	void Movement()
    {
		Vector3 deltaPosition = transform.forward * forwardSpeed;
		if (isInputHeld && !isFeverMode)
		{
			Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
			if(Physics.Raycast(ray, out RaycastHit raycastHit))
            {
				touchPosition = raycastHit.point;
			}
            if (Mathf.Abs(touchPosition.x - transform.position.x) > 0.2f)
            {
				if (touchPosition.x > transform.position.x && transform.position.x < 5.9f)
                {
					horizontalInput = 1;
					deltaPosition += transform.right * sideSpeed;
				}
                else if (touchPosition.x < transform.position.x && transform.position.x > -6)
                {
					horizontalInput = -1;
					deltaPosition -= transform.right * sideSpeed;
				}
				else
				{
					horizontalInput = 0;
				}
			}
            else
            {
				horizontalInput = 0;
			}
		}
        else
        {
			horizontalInput = 0;
		}
		
		transform.position += deltaPosition * Time.deltaTime;
	}

	private void CameraFollow()
    {
		Vector3 pos = new Vector3(_camera.transform.position.x, _camera.transform.position.y, transform.position.z + _cameraOffset);
		_camera.transform.position = pos;
	}

	private void GetInput()
	{
		if (Input.GetMouseButtonDown(0))
		{
			if (EventSystem.current.IsPointerOverGameObject())
				return;

			isInputHeld = true;
		}
		else if (Input.GetMouseButtonUp(0))
		{
			isInputHeld = false;
		}
	}

	private void SideRotation()
    {
		float horizontal = horizontalInput;
		float vertical = 0.75f;
		Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

		if(direction.magnitude >= 0.1f)
        {
			float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
			float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
			transform.rotation = Quaternion.Euler(0f, angle, 0f);
        }
	}

	void SnakeMovement()
	{
		if (forwardSpeed != 0)
		{
			if (isBodyReady && snakeBody.Count > 1)
			{
				for (int i = 1; i < snakeBody.Count; i++)
				{
					BodyPart markM = snakeBody[i - 1].GetComponent<BodyPart>();
					BodyPart.Marker closestMarker = markM.ClosestMarker(1);
					snakeBody[i].transform.position = closestMarker.position;
					snakeBody[i].transform.rotation = Quaternion.Euler(snakeBody[i].transform.eulerAngles.x, closestMarker.rotation.y, snakeBody[i].transform.eulerAngles.z);
					markM.markerList.RemoveAt(0);
				}
			}
		}
	}

	void CreateBodyParts()
	{
		isBodyReady = false;
		if (snakeBody.Count == 0)
		{
			GameObject temp1 = bodyParts[0];
			if (!temp1.GetComponent<BodyPart>())
			{
				temp1.AddComponent<BodyPart>();
			}
			snakeBody.Add(temp1);
			bodyParts.RemoveAt(0);
		}
		BodyPart markM = snakeBody[snakeBody.Count - 1].GetComponent<BodyPart>();
		if (countUp == 0)
		{
			markM.ClearMarkerList();
		}
		countUp += Time.deltaTime;
		if (countUp >= distanceBeetween)
		{
			GameObject temp = bodyParts[0];
			bodyParts[0].transform.parent = transform;
			if (!temp.GetComponent<BodyPart>())
			{
				temp.AddComponent<BodyPart>();
			}
			snakeBody.Add(temp);
			bodyParts.RemoveAt(0);
			temp.GetComponent<BodyPart>().ClearMarkerList();
			countUp = 0;
		}
		isBodyReady = true;
	}

	public IEnumerator Swallow ()
	{
		for (int i = 2; i < snakeBody.Count; i++)
		{
			StartCoroutine(snakeBody[i].GetComponent<BodyPart>().SnakeSwallow(swallowSpeed, swallowDuration));
			yield return new WaitForSeconds(0.05f);
		}
	}
}
