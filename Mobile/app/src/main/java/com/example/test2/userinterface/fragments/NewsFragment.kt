package com.example.test2.userinterface.fragments

import android.os.Bundle
import androidx.fragment.app.Fragment
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import com.example.test2.R
import kotlinx.android.synthetic.main.fragment_enter.*
import kotlinx.android.synthetic.main.fragment_news.*

class NewsFragment : Fragment(R.layout.fragment_news) {
    override fun onStart() {
        super.onStart()
        reload_news_button.setOnClickListener {reload_news_server()}
        add_news_button.setOnClickListener {add_news_server()}
    }

    private fun add_news_server() {
        TODO("Not yet implemented")
    }

    private fun reload_news_server() {
        TODO("Not yet implemented")
    }
}