using UnityEngine;
//using UnityEditor;

public class FreeCameraLook : Pivot {

	[SerializeField] private float moveSpeed = 5f;
	[SerializeField] private float turnSpeed = 1.5f;
	[SerializeField] private float turnsmoothing = .1f;
	[SerializeField] private float tiltMax = 75f;
	[SerializeField] private float tiltMin = 0f;
	[SerializeField] private bool lockCursor = false;

	private float lookAngle;
	private float tiltAngle;

    private BoxCollider col;

	private const float LookDistance = 100f;

	private float smoothX = 0;
	private float smoothY = 0;
	private float smoothXvelocity = 0;
	private float smoothYvelocity = 0;


	protected override void Awake()
	{
		base.Awake();

        Cursor.lockState = CursorLockMode.Confined;

        col = GetComponentInChildren<BoxCollider>();
		cam = GetComponentInChildren<Camera>().transform;
		pivot = cam.parent;
	}
	
    void OnCollisionEnter()
    {
        Debug.Log("hi");
    }

	// Update is called once per frame
 protected override	void Update ()
	{
		base.Update();

		HandleRotationMovement();

		if (lockCursor && Input.GetMouseButtonUp (0))
		{
            Cursor.lockState = CursorLockMode.Confined;
		}
	}

	void OnDisable()
	{
        Cursor.lockState = CursorLockMode.None;
    }

	protected override void Follow (float deltaTime)
	{
		transform.position = Vector3.Lerp(transform.position, target.position, deltaTime * moveSpeed);

	}

	void HandleRotationMovement()
	{
		float x = Input.GetAxis("ControllerXR");
		float y = Input.GetAxis("ControllerYR");
        //Debug.Log(Input.GetAxis("ControllerYR"));
        if(Mathf.Abs(x)<0.1)
        {
            x = 0;
        }
        if(Mathf.Abs(y)<0.1)
        {
            y = 0;
        }
		if (turnsmoothing > 0)
		{
			smoothX = Mathf.SmoothDamp (smoothX, x, ref smoothXvelocity, turnsmoothing);
			smoothY = Mathf.SmoothDamp (smoothY, y, ref smoothYvelocity, turnsmoothing);
				} 
		else
		{
			smoothX = x;
			smoothY = y;
				}
		lookAngle += smoothX * turnSpeed;

		transform.rotation = Quaternion.Euler(0f, lookAngle, 0);

		tiltAngle -= smoothY * turnSpeed;
		tiltAngle = Mathf.Clamp (tiltAngle, -tiltMin, tiltMax);

		pivot.localRotation = Quaternion.Euler(tiltAngle,0,0);
	}

}
