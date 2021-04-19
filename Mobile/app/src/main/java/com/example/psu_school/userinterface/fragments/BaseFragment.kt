package com.example.psu_school.userinterface.fragments

import androidx.fragment.app.Fragment
import com.example.psu_school.MainActivity

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