package com.example.test2.userinterface.fragments

import android.os.Bundle
import androidx.fragment.app.Fragment
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import com.example.test2.R
import kotlinx.android.synthetic.main.fragment_news.*
import kotlinx.android.synthetic.main.fragment_time_table.*

class TimeTableFragment : BaseFragment(R.layout.fragment_time_table) {
    override fun onStart() {
        super.onStart()
        reload_time_table_button.setOnClickListener {reload_time_table_server()}
        add_time_table_button.setOnClickListener {add_time_table_server()}
    }

    private fun add_time_table_server() {
        TODO("Not yet implemented")
    }

    private fun reload_time_table_server() {
        TODO("Not yet implemented")
    }

}