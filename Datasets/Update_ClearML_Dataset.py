from clearml import Dataset

# ds = Dataset.create(dataset_name='Dataset_OOT_V3',
#                     dataset_project='MontaOrderVerification',
#                     parent_datasets=['dc003586bc29455c9806969663f624a7'])
#
# ds.sync_folder(local_path="Dataset_OOT_V3")
#
# ds.finalize(auto_upload=True)

ds = Dataset.create(dataset_name='Dataset_OOT_V3_1280',
                    dataset_project='MontaOrderVerification')

ds.sync_folder(local_path="Dataset_OOT_V3_1280")

ds.finalize(auto_upload=True)
