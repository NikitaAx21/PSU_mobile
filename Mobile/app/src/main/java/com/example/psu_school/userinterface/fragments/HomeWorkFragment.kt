package com.example.psu_school.userinterface.fragments

import androidx.fragment.app.Fragment
import com.example.psu_school.R
import kotlinx.android.synthetic.main.fragment_home_work.*

class HomeWorkFragment : Fragment(R.layout.fragment_home_work) {
    override fun onStart() {
        super.onStart()
        reload_home_work_button.setOnClickListener {reload_home_work_server()}
        add_home_work_button.setOnClickListener {add_home_work_server()}
    }

    private fun add_home_work_server() {
        TODO("Not yet implemented")
    }

    private fun reload_home_work_server() {
        TODO("Not yet implemented")
    }
}