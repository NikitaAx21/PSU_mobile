package com.example.test2.userinterface.fragments

import android.os.Bundle
import androidx.fragment.app.Fragment
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import com.example.test2.MainActivity
import com.example.test2.R

open class BaseFragment(val layout: Int) : Fragment(layout) {
    override fun onStart() {
        super.onStart()
        (activity as MainActivity).mmyDrawer.disableDrawer()
        (activity as MainActivity).hide_key_board()
    }

    override fun onStop() {
        super.onStop()
        (activity as MainActivity).mmyDrawer.enableDrawer()
    }
}