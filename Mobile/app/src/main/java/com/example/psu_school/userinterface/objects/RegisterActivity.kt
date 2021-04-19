package com.example.psu_school.userinterface.objects

import androidx.appcompat.app.AppCompatActivity
import android.os.Bundle
import com.example.psu_school.R
import com.example.psu_school.databinding.ActivityRegisterBinding
import com.example.psu_school.userinterface.fragments.EnterFragment
import utilits.replace_fragment

class RegisterActivity : AppCompatActivity() {
    private lateinit var mBinding: ActivityRegisterBinding
    private lateinit var mToolbar: androidx.appcompat.widget.Toolbar

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        mBinding = ActivityRegisterBinding.inflate(layoutInflater)
        setContentView(mBinding.root)
    }

    override fun onStart() {
        super.onStart()
        mToolbar = mBinding.registerToolBar
        setSupportActionBar(mToolbar)
        title = getString(R.string.register_title)

        replace_fragment(EnterFragment())
    }
}