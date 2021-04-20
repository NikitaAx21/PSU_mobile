package com.example.psu_school.userinterface.fragments

import android.view.Menu
import android.view.MenuInflater
import androidx.fragment.app.Fragment
import com.example.psu_mobile.UserInfo
import com.example.psu_school.MainActivity
import com.example.psu_school.R
import kotlinx.android.synthetic.main.fragment_enter.*
import kotlinx.android.synthetic.main.fragment_settings.*
import utilits.show_toast

class SettingsFragment : Fragment(R.layout.fragment_settings) {
    //жизненный цикл
    override fun onResume() {
        super.onResume()
        setHasOptionsMenu(true)//включаем меню в контексте
    }
    //безопасный вызов активити
    override fun onCreateOptionsMenu(menu: Menu, inflater: MenuInflater) {
        activity?.menuInflater?.inflate(R.menu.settings_action_menu, menu)
    }

    override fun onStart() {
        super.onStart()
        get_settings()
        save_settings_button.setOnClickListener {save_settings()}
        change_user_photo.setOnClickListener {change_photo()}
    }

    private fun get_settings(){
        sett_label_name.text = (activity as MainActivity).mUserInfo.Name
        sett_label_surname.text = (activity as MainActivity).mUserInfo.Surname
        sett_label_futhname.text = (activity as MainActivity).mUserInfo.Futhname
        sett_label_group.text = (activity as MainActivity).mUserInfo.Group

        user_name.text = (activity as MainActivity).mUserInfo.Name + " " +
                (activity as MainActivity).mUserInfo.Surname
        user_group.text = (activity as MainActivity).mUserInfo.Group
    }

    private fun save_settings() {
        show_toast(getString(R.string.sett_save_change_text))
    }

    private fun change_photo() {
        show_toast("Фото")
    }
}