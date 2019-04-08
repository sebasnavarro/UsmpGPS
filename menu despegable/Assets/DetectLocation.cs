using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Kudan.AR.Samples;

public class DetectLocation : MonoBehaviour
{


    //Marcador ubicación para mostrar
    private Vector2 targetCoordinates;
    //Variable de ubicación del dispositivo
    private Vector2 deviceCoordinates;
    //Distancia permitida desde el dispositivo al objetivo
    private float distanceFromTarget = 0.00004f;
    //Distancia entre el dispositivo y las coordenadas del objetivo.
    private float proximity = 0.001f;
    //Los valores de latitud y largo obtención que el dispositivo consigue
    private float sLatitude, sLongitude;
    //establecer valor para la ubicación de destino
    public float dLatitude = -6.884687f, dLongitude = 107.605945f;
    //Variable para solicitud de ubicación
    private bool enableByRequest = true;
    public int maxWait = 10;
    public bool ready = false;
    public Text text;
    //sampleapp script
    public SampleApp sa;

    void Start()
    {
        
        
            targetCoordinates = new Vector2(dLatitude, dLongitude);
            StartCoroutine(getLocation());

    }

    void Update()
    {
        //StartCoroutine(getLocation());
        //startCalculate();
    }

    //obtener la ubicación de la última actualización, necesitamos latitud y longitud
    IEnumerator getLocation()
    {
        LocationService service = Input.location;
        if (!enableByRequest && !service.isEnabledByUser)
        {
            Debug.Log("Location Services not enabled by user");
            yield break;
        }
        service.Start();
        while (service.status == LocationServiceStatus.Initializing && maxWait > 0)
        {
            yield return new WaitForSeconds(1);
            maxWait--;
        }
        if (maxWait < 1)
        {
            Debug.Log("Timed out");
            yield break;
        }
        if (service.status == LocationServiceStatus.Failed)
        {
            Debug.Log("Unable to determine device location");
            yield break;
        }
        else
        {
            text.text = "Target Location : " + dLatitude + ", " + dLongitude + "\nMy Location: " + service.lastData.latitude + ", " + service.lastData.longitude;
            //Aquí pasamos los valores lat y lon desde el dispositivo
            sLatitude = service.lastData.latitude;
            sLongitude = service.lastData.longitude;
        }
        //detener el servicio si quieres
        //service.Stop();
        ready = true;
        startCalculate();
    }
    //método para calcular distancias entre la ubicación del dispositivo y la ubicación de destino
    public void startCalculate()
    {
        //crear vector2 desde dispositivo lat y lon
        deviceCoordinates = new Vector2(sLatitude, sLongitude);
        //proximidad calcular distancia
        proximity = Vector2.Distance(targetCoordinates, deviceCoordinates);
        //si la proximidad es menor o  igual objetivo
        if (proximity <= distanceFromTarget)
        {
           
            text.text = text.text + "\nDistance : " + proximity.ToString();
            text.text += "\nTarget Detected";
            //Mostrar objeto 3d! llamar sampleapp script
            sa.StartClicked();
        }
        else
        {
            //si no mostrar advertencia o lo que sea
            text.text = text.text + "\nDistance : " + proximity.ToString();
            text.text += "\nTarget not detected, too far!";
        }
    }

}
