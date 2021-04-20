package com.example.psu_school.userinterface.fragments

import android.content.Intent
import android.os.Bundle
import android.widget.Toast
import androidx.fragment.app.Fragment
import com.example.psu_school.MainActivity
import com.google.zxing.integration.android.IntentIntegrator
import kotlinx.android.synthetic.main.fragment_photo.*
import utilits.show_toast


class PhotoFragment : Fragment() {
    //private var toast: String? = null

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        IntentIntegrator.forSupportFragment(this).initiateScan()
    }

    private fun display_toast() {
        if (activity != null) {
            (activity as MainActivity).show_toast((activity as MainActivity).mUserInfo.Qrqode!!)
        }
    }

    override fun onActivityResult(requestCode: Int, resultCode: Int, data: Intent?) {
        val result = IntentIntegrator.parseActivityResult(requestCode, resultCode, data)
        if (result != null) {
            if (result.getContents() == null) {
                (activity as MainActivity).mUserInfo.Qrqode = ""
            }
            else {
                (activity as MainActivity).mUserInfo.Qrqode = result.contents
            }
            display_toast()
        }
        else {
            super.onActivityResult(requestCode, resultCode, data)
        }
    }

}