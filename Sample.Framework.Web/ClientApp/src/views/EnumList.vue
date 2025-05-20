<template>
  <div>
    <div class="table-operations">
      <a-button type="primary" @click="createEnum">
        <plus-outlined /> 新建枚举
      </a-button>
      <a-button @click="refreshList">
        <reload-outlined /> 刷新
      </a-button>
    </div>

    <a-table :columns="columns" :data-source="enums" row-key="id" :loading="loading">
      <template #bodyCell="{ column, record }">
        <template v-if="column.key === 'action'">
          <a-space>
            <a-button type="link" @click="editEnum(record.id)">
              <edit-outlined /> 编辑
            </a-button>
            <a-popconfirm
              title="确定要删除这个枚举吗？"
              ok-text="是"
              cancel-text="否"
              @confirm="deleteEnum(record.id)"
            >
              <a-button type="link" danger>
                <delete-outlined /> 删除
              </a-button>
            </a-popconfirm>
          </a-space>
        </template>
      </template>
    </a-table>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue';
import { useRouter } from 'vue-router';
import { message } from 'ant-design-vue';
import { PlusOutlined, ReloadOutlined, EditOutlined, DeleteOutlined } from '@ant-design/icons-vue';
import api from '../services/api';

const router = useRouter();
const enums = ref([]);
const loading = ref(false);

const columns = [
  {
    title: 'ID',
    dataIndex: 'id',
    key: 'id',
    width: '80px'
  },
  {
    title: '名称',
    dataIndex: 'name',
    key: 'name',
  },
  {
    title: '命名空间',
    dataIndex: 'namespace',
    key: 'namespace',
  },
  {
    title: '描述',
    dataIndex: 'description',
    key: 'description',
  },
  {
    title: '值数量',
    key: 'valuesCount',
    customRender: ({ record }) => record.values?.length || 0
  },
  {
    title: '操作',
    key: 'action',
    fixed: 'right',
    width: '150px'
  }
];

const fetchEnums = async () => {
  loading.value = true;
  try {
    const response = await api.enum.getAll();
    enums.value = response.data;
  } catch (error) {
    message.error('获取枚举列表失败');
    console.error(error);
  } finally {
    loading.value = false;
  }
};

const createEnum = () => {
  router.push('/enums/create');
};

const editEnum = (id) => {
  router.push(`/enums/${id}`);
};

const deleteEnum = async (id) => {
  try {
    await api.enum.delete(id);
    message.success('删除成功');
    await fetchEnums();
  } catch (error) {
    message.error('删除失败');
    console.error(error);
  }
};

const refreshList = () => {
  fetchEnums();
};

onMounted(() => {
  fetchEnums();
});
</script>

<style scoped>
.table-operations {
  margin-bottom: 16px;
  display: flex;
  gap: 8px;
}
</style>