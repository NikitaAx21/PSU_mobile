package com.example.psu_school.userinterface.fragments

import androidx.fragment.app.Fragment
import com.example.psu_school.R
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