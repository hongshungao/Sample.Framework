<template>
  <div>
    <div class="table-operations">
      <a-button type="primary" @click="createEntity">
        <plus-outlined /> 新建实体
      </a-button>
      <a-button @click="refreshList">
        <reload-outlined /> 刷新
      </a-button>
    </div>

    <a-table :columns="columns" :data-source="entities" row-key="id" :loading="loading">
      <template #bodyCell="{ column, record }">
        <template v-if="column.key === 'action'">
          <a-space>
            <a-button type="link" @click="editEntity(record.id)">
              <edit-outlined /> 编辑
            </a-button>
            <a-button type="link" @click="generateCode(record.id)">
              <code-outlined /> 生成代码
            </a-button>
            <a-popconfirm
              title="确定要删除这个实体吗？"
              ok-text="是"
              cancel-text="否"
              @confirm="deleteEntity(record.id)"
            >
              <a-button type="link" danger>
                <delete-outlined /> 删除
              </a-button>
            </a-popconfirm>
          </a-space>
        </template>
      </template>
    </a-table>

    <a-modal
      v-model:visible="codeModalVisible"
      title="代码生成结果"
      width="600px"
      :footer="null"
    >
      <a-result
        :status="codeGenResult.success ? 'success' : 'error'"
        :title="codeGenResult.success ? '代码生成成功' : '代码生成失败'"
      >
        <template #extra>
          <div v-if="codeGenResult.success">
            <p>生成的文件:</p>
            <a-list
              size="small"
              bordered
              :data-source="codeGenResult.files"
              :render-item="(item) => ({ children: item })"
            />
          </div>
          <div v-else>
            <p>错误信息: {{ codeGenResult.error }}</p>
          </div>
        </template>
      </a-result>
    </a-modal>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue';
import { useRouter } from 'vue-router';
import { message } from 'ant-design-vue';
import { PlusOutlined, ReloadOutlined, EditOutlined, DeleteOutlined, CodeOutlined } from '@ant-design/icons-vue';
import api from '../services/api';

const router = useRouter();
const entities = ref([]);
const loading = ref(false);
const codeModalVisible = ref(false);
const codeGenResult = ref({ success: false, files: [], error: '' });

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
    title: '属性数量',
    key: 'propertiesCount',
    customRender: ({ record }) => record.properties?.length || 0
  },
  {
    title: '操作',
    key: 'action',
    fixed: 'right',
    width: '200px'
  }
];

const fetchEntities = async () => {
  loading.value = true;
  try {
    const response = await api.entity.getAll();
    entities.value = response.data;
  } catch (error) {
    message.error('获取实体列表失败');
    console.error(error);
  } finally {
    loading.value = false;
  }
};

const createEntity = () => {
  router.push('/entities/create');
};

const editEntity = (id) => {
  router.push(`/entities/${id}`);
};

const deleteEntity = async (id) => {
  try {
    await api.entity.delete(id);
    message.success('删除成功');
    await fetchEntities();
  } catch (error) {
    message.error('删除失败');
    console.error(error);
  }
};

const generateCode = async (id) => {
  try {
    const response = await api.entity.generateCode(id);
    codeGenResult.value = {
      success: true,
      files: response.data,
      error: ''
    };
    codeModalVisible.value = true;
  } catch (error) {
    codeGenResult.value = {
      success: false,
      files: [],
      error: error.response?.data || error.message || '未知错误'
    };
    codeModalVisible.value = true;
  }
};

const refreshList = () => {
  fetchEntities();
};

onMounted(() => {
  fetchEntities();
});
</script>

<style scoped>
.table-operations {
  margin-bottom: 16px;
  display: flex;
  gap: 8px;
}
</style>