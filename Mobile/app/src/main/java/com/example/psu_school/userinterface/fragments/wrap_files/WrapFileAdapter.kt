package com.example.psu_school.userinterface.fragments.wrap_files

import android.content.Context
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import android.widget.ImageView
import android.widget.ProgressBar
import android.widget.TextView
import androidx.constraintlayout.widget.ConstraintLayout
import androidx.recyclerview.widget.RecyclerView
import com.example.psu_school.R
import com.example.psu_school.models.univarsal_model
import kotlinx.android.synthetic.main.file_wrap_item.view.*

class WrapFileAdapter : RecyclerView.Adapter<WrapFileAdapter.WrapFileHolder>() {

    private var mListFilesCache = ArrayList<univarsal_model>()

    class WrapFileHolder(view: View) : RecyclerView.ViewHolder(view){
        val blockFile: ConstraintLayout = view.wrap_item_block
        val fileName: TextView = view.wrap_item_view
    }

    override fun onCreateViewHolder(parent: ViewGroup, viewType: Int): WrapFileHolder {
        val view = LayoutInflater.from(parent.context)
                        .inflate(R.layout.file_wrap_item, parent, false)
        return WrapFileHolder(view)
    }

    override fun getItemCount(): Int = mListFilesCache.size

    override fun onBindViewHolder(holder: WrapFileHolder, position: Int) {
        holder.blockFile.visibility = View.VISIBLE
        holder.fileName.text = mListFilesCache[position].fileName
    }

    fun setList(list: ArrayList<univarsal_model>) {
        mListFilesCache = list
        notifyDataSetChanged()
    }
}