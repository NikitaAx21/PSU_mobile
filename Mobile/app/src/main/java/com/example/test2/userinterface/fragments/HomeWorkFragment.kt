package com.example.test2.userinterface.fragments

import android.os.Bundle
import androidx.fragment.app.Fragment
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import com.example.test2.R
import kotlinx.android.synthetic.main.fragment_home_work.*
import kotlinx.android.synthetic.main.fragment_news.*
import kotlinx.android.synthetic.main.fragment_news.add_news_button
import kotlinx.android.synthetic.main.fragment_news.reload_news_button

class HomeWorkFragment : BaseFragment(R.layout.fragment_home_work) {
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