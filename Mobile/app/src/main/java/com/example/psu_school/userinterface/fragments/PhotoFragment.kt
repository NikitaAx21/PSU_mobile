package com.example.psu_school.userinterface.fragments

import android.content.DialogInterface
import android.content.Intent
import android.net.Uri
import androidx.fragment.app.Fragment
import com.example.psu_school.R
import com.example.psu_school.utilits.MAIN_ACTIVITY
import com.example.psu_school.utilits.QR_CODE_REQUEST_FOLLOW
import com.google.android.material.bottomsheet.BottomSheetBehavior
import com.google.zxing.integration.android.IntentIntegrator
import kotlinx.android.synthetic.main.choise_upload_file.*
import kotlinx.android.synthetic.main.choise_yes_no.*
import kotlinx.android.synthetic.main.fragment_photo.*
import utilits.replace_fragment
import utilits.set_toolbar_info
import utilits.show_toast


class PhotoFragment : Fragment(R.layout.fragment_photo) {
    private lateinit var mDialogYesNo: BottomSheetBehavior<*>

    override fun onResume() {
        super.onResume()
        set_toolbar_info(getString(R.string.drawer_qr_code_name))
    }

    override fun onStart() {
        super.onStart()
        initBottomSheet()
        qr_get_data_button.setOnClickListener { getQRCode() }
        qr_net_button.setOnClickListener { followLink() }
    }

    private fun initBottomSheet() {
        mDialogYesNo = BottomSheetBehavior.from(bottom_sheet_yes_no)
        mDialogYesNo.state = BottomSheetBehavior.STATE_HIDDEN
    }

    private fun setQuestion() {
        mDialogYesNo.state = BottomSheetBehavior.STATE_EXPANDED
        choise_yes_button.setOnClickListener { choiseYseFun() }
        choise_no_button.setOnClickListener { choiseNoFun() }
    }

    private fun choiseYseFun() {
        intentFollowLink()
    }

    private fun choiseNoFun() {
        qr_text_data.text = ""
        mDialogYesNo.state = BottomSheetBehavior.STATE_HIDDEN
    }

    private fun intentFollowLink() {
        val browserIntent = Intent(Intent.ACTION_VIEW, Uri.parse(qr_text_data.text.toString()))
        startActivityForResult(browserIntent, QR_CODE_REQUEST_FOLLOW)
    }

    private fun followLink() {
        if (qr_text_data.text.isNotEmpty()) {
            if (qr_text_data.text.contains("https://")) {
                intentFollowLink()
            } else if (qr_text_data.text.contains("http://")) {
                setQuestion()
            } else {
                show_toast(getString(R.string.qr_scanner_link_err))
            }
        }
    }

    private fun getQRCode() {
        IntentIntegrator.forSupportFragment(this).initiateScan()
    }


    override fun onActivityResult(requestCode: Int, resultCode: Int, data: Intent?) {
        val result = IntentIntegrator.parseActivityResult(requestCode, resultCode, data)
        if (result != null) {
            if (result.getContents() == null) {
                show_toast(getString(R.string.QR_code_not_found_text))
            } else {
                MAIN_ACTIVITY.mUserInfo.QR_code = result.contents
            }
            doToast()
        } else {
            super.onActivityResult(requestCode, resultCode, data)
            if (data != null && requestCode == QR_CODE_REQUEST_FOLLOW) {
                qr_text_data.text = ""
            }
        }
    }

    private fun doToast() {
        MAIN_ACTIVITY.replace_fragment(this)
        qr_text_data.text = MAIN_ACTIVITY.mUserInfo.QR_code
    }
}