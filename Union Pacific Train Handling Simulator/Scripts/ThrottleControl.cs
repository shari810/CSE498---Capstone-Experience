using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;

public class ThrottleControl : MonoBehaviour, IDataPersistence
{
	public bool doingFailure = false;

	public string TRK_FILE = @"SimulationBuild/Routes/straight100/General Content/World/straight100.trk";
	//public string CST_FILE = @"SimulationBuild\General Content\Trains\15Car Local.cst";
	public string CST_FILE = @"SimulationBuild\General Content\Trains\Coal - LD(DP)135 Cars LL8081.cst";
	public double TIMESTEP = 30.0;
	public double START_MILEPOST = 56.0;

	private float timestepTimer = 0f;

	

	public List<SimpleSimulation.Locomotive.ThrottleNotch> currentNotches = new List<SimpleSimulation.Locomotive.ThrottleNotch>();
	public SimpleSimulation.Consist consist;
	SimpleSimulation.Track track;

	float nextLogTime = 0.25f;
	private Rigidbody2D rb;

	// List of list of locomotive numbers in order from front to back corresponding to locomotive groups
	public List<List<int>> locomotiveGroups = new List<List<int>>();

	int locomotiveGroupCount;

	int currentlySelectedLocomotiveGroup = 0;

	[SerializeField] GameObject canvas;
	[SerializeField] GameObject throttlePrefab;
	List<GameObject> throttles = new List<GameObject>();

	float startingX = 0f;
	float startingY = 0f;

	public float lengthOfTrackInMeters = 8040;  // used to control how long the level takes to win
	public float actualLengthOfTrackInMeters = 8040;  // taken from the track file
	private float lengthOfTrackInMiles;

	private Dictionary<int, int> locoIndexToCarIndex = new Dictionary<int, int>();

  private int fenceIndex = 0;

	private AudioSource brakingLight;
	private AudioSource brakingHeavy;
	private AudioSource hissSound;
	private AudioSource trainSound;
	private AudioSource clickSound;

	private bool didHiss = false;

	void Awake()
	{
		canvas = LevelManager.S.throttleControlCanvas;
		ResetForces();
		lengthOfTrackInMiles = actualLengthOfTrackInMeters / 1609.344f; // meters to miles
		string buildFolder = System.IO.Path.GetDirectoryName(Application.dataPath);



		SimpleSimulation.Consist.DIESEL_ELECTRICS_DIRECTORY = @"SimulationBuild\General Content\Trains";

		track = new SimpleSimulation.Track(TRK_FILE);

		TrainInfo trainInfo = GetComponent<TrainInfo>();
		if (trainInfo != null)
        {
			CST_FILE = @"SimulationBuild\General Content\Trains\" + trainInfo.GetName() + ".cst";
		}

		consist = new SimpleSimulation.Consist(CST_FILE);
		
		track.PlaceConsist(consist, START_MILEPOST);
		List<SimpleSimulation.Locomotive> allLocomotives = consist.GetLocomotives();
		consist.StartSimulating();

		Debug.Log("Car Count: " + consist.GetCarCount());
		Debug.Log("Milepost: " + consist.FirstCar.GetMilepost(true));
		Debug.Log("Velocity: " + consist.FirstCar.GetVelocityMPS());

		foreach (SimpleSimulation.Locomotive loco in allLocomotives)
		{
			loco.SetReverserPosition(SimpleSimulation.Locomotive.ReverserPosition.Forward);
			loco.SetThrottleNotch(SimpleSimulation.Locomotive.ThrottleNotch.Idle);
			loco.SetDynamicBrakePercent(0);
		}

		// Get locomotive groups from children
		int previousCarType = -1;
		int currentCarType;
		int locomotiveNumber = 0;
		int theSiblingIndex = 0;
		foreach (Transform car in transform.Cast<Transform>().OrderBy(t => t.GetSiblingIndex()))
		{
			currentCarType = car.GetComponent<TrainCarInfo>().GetTrainCarType();
			if (currentCarType == 2) // Treat different types of locomotives the same.
				currentCarType = 0;
			// New locomotive group encountered
			if (currentCarType != previousCarType && currentCarType != 1)
			{
				locomotiveGroups.Add(new List<int>());
				locomotiveGroups.Last().Add(locomotiveNumber);
				locoIndexToCarIndex.Add(locomotiveNumber, theSiblingIndex);
				++locomotiveNumber;
			}
			// Another locomotive within group encountered
			else if (currentCarType != 1)
			{
				locomotiveGroups.Last().Add(locomotiveNumber);
				locoIndexToCarIndex.Add(locomotiveNumber, theSiblingIndex);
				++locomotiveNumber;
			}
			previousCarType = currentCarType;
			++theSiblingIndex;
		}

		locomotiveGroupCount = locomotiveGroups.Count;

		for (int i = 0; i < locomotiveGroupCount; ++i)
		{
			currentNotches.Add(0);
            GameObject throttle = Instantiate(throttlePrefab) as GameObject;
            throttle.transform.SetParent(canvas.transform, false);
			var originalPosition = throttle.GetComponent<RectTransform>().anchoredPosition3D;

			throttle.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(originalPosition.x + i * -110, originalPosition.y, 0);

			throttles.Add(throttle);
        }

		for (int i = 0; i < throttles.Count(); i++)
        {
			AssignButtons(throttles[i], i);
        }


		//for(int i = 0; i < locomotiveGroups.Count; ++i)
		//      {
		//	foreach (var locomotiveNum in locomotiveGroups[i])
		//          {
		//		Debug.Log(i.ToString() + " " + locomotiveNum.ToString());
		//          }
		//      }

		//UpdateThrottleAndSimulate(consist, currentNotches[currentlySelectedLocomotiveGroup], 30);
		startingX = transform.position.x;
		startingY = transform.position.y;
	}

	void Start()
    {
		didHiss = false;
		hissSound = GameObject.Find("LevelEssentials/SoundBank/TRAIN/SteamHiss1").GetComponent<AudioSource>();
		brakingLight = GameObject.Find("LevelEssentials/SoundBank/TRAIN/HinkleHump_2ndRetarder").GetComponent<AudioSource>();
		brakingHeavy = GameObject.Find("LevelEssentials/SoundBank/TRAIN/HinkleHump_MainRetarder").GetComponent<AudioSource>();
		trainSound = GameObject.Find("LevelEssentials/SoundBank/BACKGROUND/DistantTrain").GetComponent<AudioSource>();
		clickSound = GameObject.Find("LevelEssentials/SoundBank/UI/Click").GetComponent<AudioSource>();
	}

    void AssignButtons(GameObject throttle, int index)
    {
		Button up = throttle.transform.GetChild(0).GetComponent<Button>();
		up.onClick.AddListener(delegate { 
			UpThrottle(throttle, index);
			clickSound.ignoreListenerPause = true;
			clickSound.Play();
		});

		Button down = throttle.transform.GetChild(1).GetComponent<Button>();
		down.onClick.AddListener(delegate { 
			DownThrottle(throttle, index);
			clickSound.ignoreListenerPause = true;
			clickSound.Play();
		});

	}

    void DoIndicatorGlow(int index)
    {
        foreach (int locoIndex in locomotiveGroups[index]) 
        {
            if (transform.GetChild(locoIndexToCarIndex[locoIndex]).childCount > 0)
            {
                transform.GetChild(locoIndexToCarIndex[locoIndex]).GetChild(0).GetComponent<IndicatorGlow>().Glow();
            }
        }
    }

    void DoUpThrottle(int index)
    {
        if ((int)currentNotches[index] < 8)
		{
			currentNotches[index] = (SimpleSimulation.Locomotive.ThrottleNotch)((int)currentNotches[index] + 1);
			List<SimpleSimulation.Locomotive> allLocos = consist.GetLocomotives();
			// Idle state
			if ((int)currentNotches[index] == 0)
			{
				foreach (int locoIndex in locomotiveGroups[index])
				{

					allLocos[locoIndex].SetThrottleNotch(currentNotches[index]);
				}
				foreach (int locoIndex in locomotiveGroups[index])
				{

					allLocos[locoIndex].SetDynamicBrakePercent(0);
				}
			}
			// Positive throttle
			else if ((int)currentNotches[index] > 0)
			{
				foreach (int locoIndex in locomotiveGroups[index])
				{

					allLocos[locoIndex].SetThrottleNotch(currentNotches[index]);
				}
			}
			// Dynamic braking
			else
			{
				foreach (int locoIndex in locomotiveGroups[index])
				{

					allLocos[locoIndex].SetDynamicBrakePercent(((int)currentNotches[index]) * -12.5);
				}
			}

		}
    }

    void DoDownThrottle(int index)
    {
        if ((int)currentNotches[index] > -8)
		{
			currentNotches[index] = (SimpleSimulation.Locomotive.ThrottleNotch)((int)currentNotches[index] - 1);
			List<SimpleSimulation.Locomotive> allLocos = consist.GetLocomotives();
			// Idle state
			if ((int)currentNotches[index] == 0)
			{
				foreach (int locoIndex in locomotiveGroups[index])
				{

					allLocos[locoIndex].SetThrottleNotch(currentNotches[index]);
				}
				foreach (int locoIndex in locomotiveGroups[index])
				{

					allLocos[locoIndex].SetDynamicBrakePercent(0);
				}
			}
			// Positive throttle
			else if ((int)currentNotches[index] > 0)
			{
				foreach (int locoIndex in locomotiveGroups[index])
				{

					allLocos[locoIndex].SetThrottleNotch(currentNotches[index]);
				}
			}
			// Dynamic braking
			else
			{
				if ((int)currentNotches[index] == -1 && !brakingLight.isPlaying && consist.FirstCar.GetVelocityMPS() > 1)
                {
					brakingLight.ignoreListenerPause = true;
					brakingLight.Play();
                }
				else if ((int)currentNotches[index] == -8 && !brakingLight.isPlaying && !brakingHeavy.isPlaying && consist.FirstCar.GetVelocityMPS() > 1)
                {
					brakingHeavy.ignoreListenerPause = true;
					brakingHeavy.Play();
                }
				foreach (int locoIndex in locomotiveGroups[index])
				{

					allLocos[locoIndex].SetDynamicBrakePercent(((int)currentNotches[index]) * -12.5);
				}
			}
		}
    }

	void UpThrottle(GameObject throttle, int index)
    {
        if (!(fenceIndex == -1 && index == 1 && locomotiveGroups.Count > 1) && !(fenceIndex == 1 && index == 0 && locomotiveGroups.Count > 1))
        {
            DoIndicatorGlow(index);
            DoUpThrottle(index);
            Debug.Log("Current throttle on " + (index) + ": " + currentNotches[index]);
		    UpdateThrottleText(throttles[index], index);
        }
		
        
        if (fenceIndex == -1 && index == 0 && locomotiveGroups.Count > 1)
        {
            DoIndicatorGlow(index + 1);
            DoUpThrottle(index + 1);
            Debug.Log("Current throttle on " + (index + 1) + ": " + currentNotches[index + 1]);
		    UpdateThrottleText(throttles[index + 1], index + 1);
        }

        else if (fenceIndex == 1 && index == 1 && locomotiveGroups.Count > 1)
        {
            DoIndicatorGlow(index - 1);
            DoUpThrottle(index - 1);
            Debug.Log("Current throttle on " + (index - 1) + ": " + currentNotches[index - 1]);
		    UpdateThrottleText(throttles[index - 1], index - 1);
        }
	}

	void DownThrottle(GameObject throttle, int index)
	{
		if (!(fenceIndex == -1 && index == 1 && locomotiveGroups.Count > 1) && !(fenceIndex == 1 && index == 0 && locomotiveGroups.Count > 1))
        {
            DoIndicatorGlow(index);
            DoDownThrottle(index);
            Debug.Log("Current throttle on " + (index) + ": " + currentNotches[index]);
		    UpdateThrottleText(throttles[index], index);
        }
		
        
        if (fenceIndex == -1 && index == 0 && locomotiveGroups.Count > 1)
        {
            DoIndicatorGlow(index + 1);
            DoDownThrottle(index + 1);
            Debug.Log("Current throttle on " + (index + 1) + ": " + currentNotches[index + 1]);
		    UpdateThrottleText(throttles[index + 1], index + 1);
        }

        else if (fenceIndex == 1 && index == 1 && locomotiveGroups.Count > 1)
        {
            DoIndicatorGlow(index - 1);
            DoDownThrottle(index - 1);
            Debug.Log("Current throttle on " + (index - 1) + ": " + currentNotches[index - 1]);
		    UpdateThrottleText(throttles[index - 1], index - 1);
        }
	}

	void UpdateThrottleText(GameObject throttle, int index)
    {
		Text throtText = throttle.transform.GetChild(3).GetComponent<Text>();
		var currentNotch = currentNotches[index];
		bool prependDB = false;
		if (currentNotch < 0)
        {
			prependDB = true;
        }
		throtText.text = prependDB ? "DB" + currentNotches[index].ToString().Substring(1) : currentNotches[index].ToString();
	}

	// Update is called once per frame
	void Update()
	{
    fenceIndex = MoveFence.fenceValue;
		if (!didHiss && consist.FirstCar.GetVelocityMPS() > 1.7)
        {
			hissSound.ignoreListenerPause = true;
			hissSound.Play();
			didHiss = true;
        }
		if (!trainSound.isPlaying && consist.FirstCar.GetVelocityMPS() > 1 && !(GameManager.Victory || GameManager.GameisOver || GameManager.isPaused))
        {
			trainSound.ignoreListenerPause = true;
			trainSound.Play();
		}
		else if ((trainSound.isPlaying && consist.FirstCar.GetVelocityMPS() <= 1) || GameManager.Victory || GameManager.GameisOver || GameManager.isPaused)
        {
			if (GameManager.isPaused)
				trainSound.Pause();
			else
				trainSound.Stop();
        }
		else if (trainSound.isPlaying && consist.FirstCar.GetVelocityMPS() > 1)
        {
			trainSound.volume = Mathf.Lerp(0.05f, 0.25f, ((float)consist.FirstCar.GetVelocityMPS() - 1) / 12.4112f);
        }

		//Debug.Log(currentlySelectedLocomotiveGroup);
		// Select right locomotive group (default is front--rightmost)
		/*if (Input.GetKeyDown(KeyCode.D))
		{
			if (currentlySelectedLocomotiveGroup > 0)
				--currentlySelectedLocomotiveGroup;
			Debug.Log("Current throttle on " + currentlySelectedLocomotiveGroup + ": " + currentNotches[currentlySelectedLocomotiveGroup]);
		}
		// Select left locomotive group (default is front--rightmost)
		else if (Input.GetKeyDown(KeyCode.A))
		{
			if (currentlySelectedLocomotiveGroup < locomotiveGroupCount - 1)
				++currentlySelectedLocomotiveGroup;
			Debug.Log("Current throttle on " + currentlySelectedLocomotiveGroup + ": " + currentNotches[currentlySelectedLocomotiveGroup]);
		}
		else if (Input.GetKeyDown(KeyCode.T))
		{
			if ((int)currentNotches[currentlySelectedLocomotiveGroup] < 8)
			{
				currentNotches[currentlySelectedLocomotiveGroup] = (SimpleSimulation.Locomotive.ThrottleNotch)((int)currentNotches[currentlySelectedLocomotiveGroup] + 1);
				List<SimpleSimulation.Locomotive> allLocos = consist.GetLocomotives();
				// Idle state
				if ((int)currentNotches[currentlySelectedLocomotiveGroup] == 0)
				{
					foreach (int locoIndex in locomotiveGroups[currentlySelectedLocomotiveGroup])
					{

						allLocos[locoIndex].SetThrottleNotch(currentNotches[currentlySelectedLocomotiveGroup]);
					}
					foreach (int locoIndex in locomotiveGroups[currentlySelectedLocomotiveGroup])
					{

						allLocos[locoIndex].SetDynamicBrakePercent(0);
					}
				}
				// Positive throttle
				else if ((int)currentNotches[currentlySelectedLocomotiveGroup] > 0)
				{
					foreach (int locoIndex in locomotiveGroups[currentlySelectedLocomotiveGroup])
					{

						allLocos[locoIndex].SetThrottleNotch(currentNotches[currentlySelectedLocomotiveGroup]);
					}
				}
				// Dynamic braking
				else
				{
					foreach (int locoIndex in locomotiveGroups[currentlySelectedLocomotiveGroup])
					{

						allLocos[locoIndex].SetDynamicBrakePercent(((int)currentNotches[currentlySelectedLocomotiveGroup]) * -12.5);
					}
				}
			}
			Debug.Log("Current throttle on " + currentlySelectedLocomotiveGroup + ": " + currentNotches[currentlySelectedLocomotiveGroup]);
		}
		else if (Input.GetKeyDown(KeyCode.B))
		{
			if ((int)currentNotches[currentlySelectedLocomotiveGroup] > -8)
			{
				currentNotches[currentlySelectedLocomotiveGroup] = (SimpleSimulation.Locomotive.ThrottleNotch)((int)currentNotches[currentlySelectedLocomotiveGroup] - 1);
				List<SimpleSimulation.Locomotive> allLocos = consist.GetLocomotives();
				// Idle state
				if ((int)currentNotches[currentlySelectedLocomotiveGroup] == 0)
				{
					foreach (int locoIndex in locomotiveGroups[currentlySelectedLocomotiveGroup])
					{

						allLocos[locoIndex].SetThrottleNotch(currentNotches[currentlySelectedLocomotiveGroup]);
					}
					foreach (int locoIndex in locomotiveGroups[currentlySelectedLocomotiveGroup])
					{

						allLocos[locoIndex].SetDynamicBrakePercent(0);
					}
				}
				// Positive throttle
				else if ((int)currentNotches[currentlySelectedLocomotiveGroup] > 0)
				{
					foreach (int locoIndex in locomotiveGroups[currentlySelectedLocomotiveGroup])
					{

						allLocos[locoIndex].SetThrottleNotch(currentNotches[currentlySelectedLocomotiveGroup]);
					}
				}
				// Dynamic braking
				else
				{
					foreach (int locoIndex in locomotiveGroups[currentlySelectedLocomotiveGroup])
					{

						allLocos[locoIndex].SetDynamicBrakePercent(((int)currentNotches[currentlySelectedLocomotiveGroup]) * -12.5);
					}
				}
			}
			Debug.Log("Current throttle on " + currentlySelectedLocomotiveGroup + ": " + currentNotches[currentlySelectedLocomotiveGroup]);
		}*/

		//if (timestepTimer * 1000.0 >= TIMESTEP)
		//{
		//	consist.Simulate(TIMESTEP);
		//	timestepTimer = 0f;
		//}

		//timestepTimer += Time.deltaTime;
		consist.Simulate(Time.deltaTime * 1000);

		// Why does trying to get the throttle notch throw an error? Could this be a bug with the Physics API?
		//if (Time.time > nextLogTime)
		//{
		//	nextLogTime += 0.25f;
		//	List<SimpleSimulation.Locomotive> allLocos = consist.GetLocomotives();
		//	for (int i = 0; i < allLocos.Count; ++i )
		//          {
		//		Debug.Log(string.Format("Locomotive {0}'s Throttle: {1}", i, allLocos[i].GetThrottleNotch().ToString()));
		//          }
		//}
		if (Time.time > nextLogTime)
		{
			//Debug.Log(string.Format("Milepost: {0}    Velocity: {1}", consist.FirstCar.GetMilepost(true), consist.FirstCar.GetVelocityMPS()));
		}

		// Set all velocities and forces
		double[] interactionForces = GetInteractionForces(consist);
		//Debug.Log(interactionForces.Length);
		int index = 0;
		foreach (Transform car in transform.Cast<Transform>().OrderBy(t => t.GetSiblingIndex()))
		{
			//car.GetComponent<SmoothTrainMovement>().movementSpeed = (float)consist.FirstCar.GetVelocityMPS();
			// Exclude last car (see description of interaction forces)
			if (index < consist.GetCarCount() - 1)
            {
				car.GetComponent<ColorGradient>().forces = (float)interactionForces[index];
				if (doingFailure)
					car.GetComponent<FailCheck>().forces = (float)interactionForces[index];
			}
			++index;
		}

        //Debug.Log(track.GetMilepost(consist, false));
        //if (track.GetElevation(track.GetMilepost(consist, false)) > 1)
        //{
        //    Debug.Log("Elevation:" + track.GetElevation(track.GetMilepost(consist, false)));
        //}
        transform.position = new Vector2(startingX + lengthOfTrackInMeters*((float)track.GetMilepost(consist, false)-(float)START_MILEPOST)/lengthOfTrackInMiles, startingY);// + (float)track.GetElevation(track.GetMilepost(consist, false)));

		//Debug.Log(((float)track.GetMilepost(consist, false)-(float)START_MILEPOST)/lengthOfTrackInMiles);

		// ON LEVEL SAVE, SAVE THIS VALUE! --> track.GetMilePost(consist, false)
		track.GetMilepost(consist, false);
	}

	public void LoadData(GameData data)
	{
		if (data.start_milepost != 0)
		{

			//this.START_MILEPOST = data.start_milepost;
		}
	}

	public void SaveData(ref GameData data)
	{
		//data.start_milepost = this.track.GetMilepost(consist, false); ;
	}

	public void ResetForces()
    {
		////consist.StartSimulating();
		//List<SimpleSimulation.Locomotive> allLocos = consist.GetLocomotives();
		//foreach (SimpleSimulation.Locomotive loco in allLocos)
		//{
		//    loco.SetThrottleNotch(0);
		//    loco.SetDynamicBrakePercent(100);
		//}
		//consist.Simulate(30000);
		//foreach (SimpleSimulation.Locomotive loco in allLocos)
		//{
		//    loco.SetDynamicBrakePercent(0);
		//}
		//consist.Simulate(30000);

		////for (SimpleSimulation.Car car = consist.FirstCar; car.Next != null; car = car.Next)
		////{
		////	//CarInteraction.InteractionSetForce(car.innerCar.carInteraction.innerInteraction, 0);
		////	CarInteraction.InteractionSetMaxForce(car.innerCar.carInteraction.innerInteraction, 0);
		////}
		SimpleSimulation.Consist.Reset();
    }

    void UpdateThrottleAndSimulate(SimpleSimulation.Consist consist, SimpleSimulation.Locomotive.ThrottleNotch throttleNotch, int numTimesteps)
	{
		List<SimpleSimulation.Locomotive> allLocos = consist.GetLocomotives();


		Debug.Log(string.Format("Throttle: {0}    Milliseconds: {1}", throttleNotch.ToString(), (numTimesteps * TIMESTEP)));
		foreach (SimpleSimulation.Locomotive loco in allLocos)
		{
			loco.SetThrottleNotch(throttleNotch);
		}
		for (int i = 0; i < numTimesteps; i++)
		{
			consist.Simulate(TIMESTEP);
		}
		Debug.Log(string.Format("Milepost: {0}    Velocity: {1}", consist.FirstCar.GetMilepost(true), consist.FirstCar.GetVelocityMPS()));
	}

	void UpdateDbAndSimulate(SimpleSimulation.Consist consist, double dbPercent, int numTimesteps)
	{
		List<SimpleSimulation.Locomotive> allLocos = consist.GetLocomotives();

		Debug.Log(string.Format("DB: {0}    Milliseconds: {1}", dbPercent, (numTimesteps * TIMESTEP)));
		foreach (SimpleSimulation.Locomotive loco in allLocos)
		{
			loco.SetDynamicBrakePercent(dbPercent);
		}
		for (int i = 0; i < numTimesteps; i++)
		{
			consist.Simulate(TIMESTEP);
		}
		Debug.Log(string.Format("Milepost: {0}    Velocity: {1}", consist.FirstCar.GetMilepost(true), consist.FirstCar.GetVelocityMPS()));
	}

	double[] GetInteractionForces(SimpleSimulation.Consist consist)
	{
		double[] forces = new double[consist.GetCarCount() - 1];
		int i = 0;
		for (SimpleSimulation.Car car = consist.FirstCar; car.Next != null; car = car.Next)
		{
			forces[i++] = car.GetInteractionForce();
		}
		return forces;
	}

	double[] GetElevationProfile(SimpleSimulation.Track track, SimpleSimulation.Consist consist, double milesBefore, double milesAfter)
	{
		double consistMilepost = track.GetMilepost(consist, true);
		double beginMilepost = consistMilepost - milesBefore;
		double endMilepost = consistMilepost + milesAfter;

		double[] elevationProfile = new double[500];
		track.GetElevation(beginMilepost, endMilepost, ref elevationProfile);
		return elevationProfile;
	}
}
