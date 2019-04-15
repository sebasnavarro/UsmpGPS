using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Kudan.AR.Samples;

public class DetectLocation : MonoBehaviour {
	
	private Vector2 targetCoordinates;
	private Vector2 deviceCoordinates;
	private float distanceFromTarget = 0.00004f;
	private float proximity = 0.001f;
	private float sLatitude, sLongitude;
	public float dLatitude = -6.884687f, dLongitude = 107.605945f;
	private bool enableByRequest = true;
	public int maxWait = 10;
	public bool ready = false;
	public Text text;
	public SampleApp sa;
    IEnumerator coroutine;


    void Start(){
		targetCoordinates = new Vector2 (dLatitude, dLongitude);
		StartCoroutine (getLocation ());
	}

	IEnumerator getLocation(){
        coroutine = updateGPS();
        LocationService service = Input.location;

		if (!enableByRequest && !service.isEnabledByUser) {
			Debug.Log("Servicios de localización no habilitados por el usuario");
			yield break;
		}
		service.Start();
		while (service.status == LocationServiceStatus.Initializing && maxWait > 0) {
			yield return new WaitForSeconds(1);
			maxWait--;
		}
		if (maxWait < 1){
			Debug.Log("Se acabo el tiempo");
			yield break;
		}
		if (service.status == LocationServiceStatus.Failed) {
			Debug.Log("No se puede determinar la ubicación del dispositivo");
			yield break;
		} else {
           	text.text = "Ubicación del objetivo : "+dLatitude + ", "+dLongitude+"\nMi ubicación: " + service.lastData.latitude + ", " + service.lastData.longitude;
			sLatitude = service.lastData.latitude;
			sLongitude = service.lastData.longitude;
            StartCoroutine(coroutine);
        }

		//service.Stop();
		ready = true;
		startCalculate ();
	}


    IEnumerator updateGPS()
    {
        float UPDATE_TIME = 3f; //Every  3 seconds
        WaitForSeconds updateTime = new WaitForSeconds(UPDATE_TIME);

        while (true)
        {
            print("Location: " + Input.location.lastData.latitude + " " + Input.location.lastData.longitude + " " + Input.location.lastData.altitude + " " + Input.location.lastData.horizontalAccuracy + " " + Input.location.lastData.timestamp);
            //longitudeText.text = "Longitude: " + Input.location.lastData.longitude;
            //latitudeText.text = "Latitude: " + Input.location.lastData.latitude;
            yield return updateTime;
        }
    }


    void stopGPS()
    {
        Input.location.Stop();
        StopCoroutine(coroutine);
    }

    void OnDisable()
    {
        stopGPS();
    }

    public void startCalculate(){
		deviceCoordinates = new Vector2 (sLatitude, sLongitude);
		proximity = Vector2.Distance (targetCoordinates, deviceCoordinates);
		if (proximity <= distanceFromTarget) {
			text.text = text.text + "\nDistance : " + proximity.ToString ();
			text.text += "\nObjeto detectado";
			sa.StartClicked ();
		} else {
			text.text = text.text + "\nDistance : " + proximity.ToString ();
			text.text += "\nObjeto no detecado, muy lejos!";
		}
	}
}
