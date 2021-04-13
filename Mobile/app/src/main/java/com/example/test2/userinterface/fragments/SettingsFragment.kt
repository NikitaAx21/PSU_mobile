package com.example.test2.userinterface.fragments

import android.view.Menu
import android.view.MenuInflater
import com.example.test2.R

class SettingsFragment : BaseFragment(R.layout.fragment_settings) {
    //жизненный цикл
    override fun onResume() {
        super.onResume()
        setHasOptionsMenu(true)//включаем меню в контексте
    }
    //безопасный вызов активити
    override fun onCreateOptionsMenu(menu: Menu, inflater: MenuInflater) {
        activity?.menuInflater?.inflate(R.menu.settings_action_menu, menu)
    }
}