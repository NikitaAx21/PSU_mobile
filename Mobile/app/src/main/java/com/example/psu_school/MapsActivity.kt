package com.example.psu_school

import android.Manifest
import android.annotation.SuppressLint
import android.content.pm.PackageManager
import android.location.Location
import androidx.appcompat.app.AppCompatActivity
import android.os.Bundle

import com.google.android.gms.maps.CameraUpdateFactory
import com.google.android.gms.maps.GoogleMap
import com.google.android.gms.maps.OnMapReadyCallback
import com.google.android.gms.maps.SupportMapFragment
import com.google.android.gms.maps.model.LatLng
import com.google.android.gms.maps.model.MarkerOptions
import android.widget.*
import androidx.core.app.ActivityCompat
import com.example.psu_mobile.UserInfo
import com.example.psu_school.R
import com.example.psu_school.utilits.MAIN_ACTIVITY
import com.google.android.gms.location.FusedLocationProviderClient
import com.google.android.gms.location.LocationServices
import kotlinx.android.synthetic.main.activity_maps.*
import utilits.show_toast

public class MapsActivity : AppCompatActivity(), OnMapReadyCallback {

    private var map: GoogleMap? = null
    private var fusedLocationProviderClient: FusedLocationProviderClient? = null
    private var lastKnownLocation: Location? = null

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_maps)

        if (ActivityCompat.checkSelfPermission(
                this,
                Manifest.permission.ACCESS_FINE_LOCATION
            ) != PackageManager.PERMISSION_GRANTED
            && ActivityCompat.checkSelfPermission(
                this,
                Manifest.permission.ACCESS_COARSE_LOCATION
            ) != PackageManager.PERMISSION_GRANTED
            && ActivityCompat.checkSelfPermission(
                this,
                Manifest.permission.ACCESS_BACKGROUND_LOCATION
            ) != PackageManager.PERMISSION_GRANTED
        ) {
            val permissions = arrayOf(
                Manifest.permission.ACCESS_FINE_LOCATION,
                Manifest.permission.ACCESS_COARSE_LOCATION,
                Manifest.permission.ACCESS_BACKGROUND_LOCATION
            )
            ActivityCompat.requestPermissions(this, permissions, 0)
        }
        // Obtain the SupportMapFragment and get notified when the map is ready to be used.
        if (getString(R.string.google_maps_key).isEmpty()) {
            Toast.makeText(this, "No API Key", Toast.LENGTH_LONG).show()
        }
        // Get the SupportMapFragment and request notification when the map is ready to be used.
        val mapFragment = supportFragmentManager.findFragmentById(R.id.map) as SupportMapFragment
        mapFragment.getMapAsync(this)

        fusedLocationProviderClient = LocationServices.getFusedLocationProviderClient(this)
    }

    /**
     * Manipulates the map once available.
     * This callback is triggered when the map is ready to be used.
     * This is where we can add markers or lines, add listeners or move the camera. In this case,
     * we just add a marker near Sydney, Australia.
     * If Google Play services is not installed on the device, the user will be prompted to install
     * it inside the SupportMapFragment. This method will only be triggered once the user has
     * installed Google Play services and returned to the app.
     */
    override fun onMapReady(googleMap: GoogleMap) {
        map = googleMap
        getDeviceLocation()
    }

    @SuppressLint("SetTextI18n")
    private fun getDeviceLocation() {
        try {
            val locationResult = fusedLocationProviderClient?.lastLocation
            locationResult?.addOnCompleteListener(this) { task ->
                if (task.isSuccessful) {
                    lastKnownLocation = task.result
                    lastKnownLocation?.let {
                        map?.moveCamera(
                            CameraUpdateFactory.newLatLng(
                                LatLng(
                                    it.latitude,
                                    it.longitude
                                )
                            )
                        )
                        MAIN_ACTIVITY.mUserInfo.Latitude = lastKnownLocation!!.latitude
                        MAIN_ACTIVITY.mUserInfo.Longitude = lastKnownLocation!!.longitude
                        map?.addMarker(
                            MarkerOptions().position(LatLng(it.latitude, it.longitude))
                                .title("Вы здесь!")
                        )
                        send_coordinates_button.setOnClickListener { send_data_to_server() }
                    }
                } else {
                    map?.uiSettings?.isMyLocationButtonEnabled = false
                }
            }
        } catch (e: SecurityException) {
            //Log.e("Exception: %s", e.message, e)
        }
    }

    private fun send_data_to_server() {
        coordinates_text_name.setText(
            "Широта: " + MAIN_ACTIVITY.mUserInfo.Latitude.toString()
                    + " Долгота: " + MAIN_ACTIVITY.mUserInfo.Longitude.toString()
        )
    }
}