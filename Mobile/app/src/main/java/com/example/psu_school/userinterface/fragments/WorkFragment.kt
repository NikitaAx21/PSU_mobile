package com.example.psu_school.userinterface.fragments

import android.app.Activity
import android.content.Intent
import android.net.Uri
import android.os.StrictMode
import androidx.fragment.app.Fragment
import androidx.recyclerview.widget.LinearLayoutManager
import androidx.recyclerview.widget.RecyclerView
import com.example.psu_mobile.AuthHelper
import com.example.psu_mobile.Request
import com.example.psu_mobile.RequestProcessor
import com.example.psu_mobile.UserInfo
import com.example.psu_school.R
import com.example.psu_school.models.this_user
import com.example.psu_school.models.univarsal_model
import com.example.psu_school.userinterface.fragments.wrap_files.WrapFileAdapter
import com.example.psu_school.utilits.CHOISE_FILE_REQUEST_CODE
import com.example.psu_school.utilits.CHOISE_IMAGE_REQUEST_CODE
import com.example.psu_school.utilits.MAIN_ACTIVITY
import com.google.android.material.bottomsheet.BottomSheetBehavior
import com.squareup.moshi.Moshi
import com.squareup.moshi.kotlin.reflect.KotlinJsonAdapterFactory
import com.squareup.picasso.Picasso
import com.theartofdev.edmodo.cropper.CropImage
import kotlinx.android.synthetic.main.choise_upload_file.*
import kotlinx.android.synthetic.main.file_wrap_item.*
import kotlinx.android.synthetic.main.fragment_work.*
import utilits.*
import java.io.ByteArrayInputStream
import java.net.URL
import kotlin.random.Random


class WorkFragment : Fragment(R.layout.fragment_work) {
    private lateinit var mBottomSheetBehavior: BottomSheetBehavior<*>

    private lateinit var mFileData: univarsal_model
    private var mList = ArrayList<univarsal_model>()
    private lateinit var mRecycler: RecyclerView
    private lateinit var mAdapter: WrapFileAdapter
    private var Index: Int = 0

    override fun onResume() {
        super.onResume()
        set_toolbar_info(getString(R.string.drawer_work_name))
        initRecycleView()
    }

    override fun onPause() {
        super.onPause()
        unset_toolbar_info()
    }

    override fun onStart() {
        super.onStart()
        initBottomSheet()
        reload_news_button.setOnClickListener { reloadNewsServer() }
        add_news_button.setOnClickListener { attach() }
    }

    private fun initRecycleView() {
        mRecycler = news_recycle
        mAdapter = WrapFileAdapter()
        mFileData = univarsal_model(id = "", fileName = "")
        mRecycler.adapter = mAdapter
        mAdapter.setList(mList)
        mRecycler.layoutManager = LinearLayoutManager(MAIN_ACTIVITY)
    }

    private fun initBottomSheet() {
        mBottomSheetBehavior = BottomSheetBehavior.from(bottom_sheet)
        mBottomSheetBehavior.state = BottomSheetBehavior.STATE_HIDDEN
    }


    private fun attach() {
        mBottomSheetBehavior.state = BottomSheetBehavior.STATE_EXPANDED
        choise_file_button.setOnClickListener { attachFile() }
        choise_image_button.setOnClickListener { attachImage() }
    }

    private fun attachFile() {
        val intent = Intent(Intent.ACTION_GET_CONTENT)
        intent.type = "*/*"
        startActivityForResult(intent, CHOISE_FILE_REQUEST_CODE)
    }

    private fun attachImage() {
        CropImage.activity()
            .setAspectRatio(1, 1)
            .setRequestedSize(250, 250)
            .start(MAIN_ACTIVITY, this)
    }

    private fun savePhotoOnDownload(){
        
    }

    private fun reloadNewsServer() {
        TODO("Not yet implemented")
    }

    private fun setDataOnScreen(uri: Uri) {
        mFileData?.id = Index.toString()
        mFileData?.fileName = get_filename_from_uri(uri)
        mList.add(Index, mFileData)
        mAdapter.setList(mList)
        mRecycler.smoothScrollToPosition(mAdapter.itemCount)
        Index += 1
    }

    override fun onActivityResult(requestCode: Int, resultCode: Int, data: Intent?) {
        super.onActivityResult(requestCode, resultCode, data)
        if (data != null && resultCode == Activity.RESULT_OK) {
            var uri: Uri?
            when (requestCode) {
                CHOISE_FILE_REQUEST_CODE -> {
                    uri = data?.data
                    setDataOnScreen(uri!!)
                }
                CHOISE_IMAGE_REQUEST_CODE -> {
                    uri = CropImage.getActivityResult(data).uri
                    setDataOnScreen(uri!!)
                }
            }
            //загрузить на сервер
        }
    }

    fun sendPostRequestAsync() {
        try {
            val mURL = URL("http://192.168.0.3:8888/")

            val policy = StrictMode.ThreadPolicy.Builder().permitAll().build()
            StrictMode.setThreadPolicy(policy)

            val authHelper = AuthHelper()
            //данные
            val mUser = this_user()
            //mUser.Login = "User_" + mUser.Surname + "_"+ {Random.nextInt()}
            mUser.Login = mUser.Surname + "_" + { Random.nextInt() }
            mUser.Password = authHelper.hashPassword(mUser.Password)

            val jsonAdapter = Moshi.Builder().add(KotlinJsonAdapterFactory()).build().adapter(this_user::class.java)
            val data = jsonAdapter.toJson(mUser)

            val request = Request()
            request.User = authHelper.getSuperUserInfo()
            request.Method = "CreateUser"

            var content = ByteArrayInputStream(data.toByteArray())

            val response = RequestProcessor().sendRequest(request, content, mURL)

        } catch (e: NumberFormatException) {
            throw RuntimeException(getString(R.string.no_server_text), e)
        }
    }
}