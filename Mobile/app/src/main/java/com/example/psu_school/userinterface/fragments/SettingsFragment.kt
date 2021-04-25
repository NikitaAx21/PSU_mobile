package com.example.psu_school.userinterface.fragments

import android.app.Activity.RESULT_OK
import android.content.Intent
import android.view.*
import android.widget.PopupMenu
import androidx.fragment.app.Fragment
import com.example.psu_school.R
import com.example.psu_school.utilits.MAIN_ACTIVITY
import com.theartofdev.edmodo.cropper.CropImage
import com.theartofdev.edmodo.cropper.CropImageView
import kotlinx.android.synthetic.main.fragment_settings.*
import utilits.*


class SettingsFragment : Fragment(R.layout.fragment_settings) {
    //жизненный цикл
    override fun onResume() {
        super.onResume()
        set_toolbar_info(getString(R.string.drawer_settings_name))
        setHasOptionsMenu(true)//включаем меню в контексте
    }

    override fun onStop() {
        super.onStop()
        hide_key_board()
    }

    //безопасный вызов popup
    override fun onCreateOptionsMenu(menu: Menu, inflater: MenuInflater) {
        activity?.menuInflater?.inflate(R.menu.settings_action_menu, menu)
    }

    override fun onOptionsItemSelected(item: MenuItem): Boolean {
        when (item.itemId) {
            R.id.settings_action_exit -> replace_fragment(EnterFragment())
        }
        return true
    }

    override fun onPause() {
        super.onPause()
        hide_key_board()
        unset_toolbar_info()
    }

    override fun onStart() {
        super.onStart()
        getSettings()
        save_settings_button.setOnClickListener { saveSettings() }
        change_user_photo.setOnClickListener { changePhoto() }
        sett_new_pass_button.setOnClickListener { changeAuth() }
    }

    private fun changeAuth() {
        if(sett_old_pass_input.text.isEmpty()){
            show_toast(getString(R.string.sett_set_old_pass))
        }
        else if(!sett_old_pass_input.text.toString().equals(MAIN_ACTIVITY.mUserInfo.Password)){
            show_toast(getString(R.string.sett_old_pass_err))
        }
        else{
            MAIN_ACTIVITY.mUserInfo.Password = set_string(sett_new_pass_input)
        }
    }

    private fun getSettings() {
        sett_name_input.setText(MAIN_ACTIVITY.mUserInfo.Name)
        sett_surname_input.setText(MAIN_ACTIVITY.mUserInfo.Surname)
        sett_midname_input.setText(MAIN_ACTIVITY.mUserInfo.Midname)
        sett_group_input.setText(MAIN_ACTIVITY.mUserInfo.Group)

        user_name.text = MAIN_ACTIVITY.mUserInfo.Name + " " +
                MAIN_ACTIVITY.mUserInfo.Surname
        user_group.text = MAIN_ACTIVITY.mUserInfo.Group
    }

    private fun saveSettings() {
        MAIN_ACTIVITY.mUserInfo.Name = set_string(sett_name_input)
        MAIN_ACTIVITY.mUserInfo.Surname = set_string(sett_surname_input)
        MAIN_ACTIVITY.mUserInfo.Midname = set_string(sett_midname_input)
        MAIN_ACTIVITY.mUserInfo.Group = set_string(sett_group_input)
        MAIN_ACTIVITY.mmyDrawer.updateHeader()
        if (is_user_data_null(MAIN_ACTIVITY.mUserInfo)) show_toast(getString(R.string.sett_save_change_text))
    }

    private fun changePhoto() {
        CropImage.activity()
            .setAspectRatio(1, 1)
            .setRequestedSize(600, 600)
            .setCropShape(CropImageView.CropShape.OVAL)
            .start(MAIN_ACTIVITY, this)
    }

    override fun onActivityResult(requestCode: Int, resultCode: Int, data: Intent?) {
        super.onActivityResult(requestCode, resultCode, data)
        if (resultCode === RESULT_OK
            && requestCode === CropImage.CROP_IMAGE_ACTIVITY_REQUEST_CODE && data != null
        ) {
            val uri = CropImage.getActivityResult(data).uri
            MAIN_ACTIVITY.mUserInfo.Url_file = uri.toString()
            this.user_photo.set_image(MAIN_ACTIVITY.mUserInfo.Url_file)
            MAIN_ACTIVITY.mmyDrawer.updateHeader()
        } else if (resultCode == CropImage.CROP_IMAGE_ACTIVITY_RESULT_ERROR_CODE)
            show_toast(getString(R.string.sett_user_photo_crop_err))
        else
            show_toast(getString(R.string.sett_change_photo_err))
    }
}