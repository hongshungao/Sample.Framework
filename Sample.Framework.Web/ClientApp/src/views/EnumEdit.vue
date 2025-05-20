<template>
  <div>
    <a-page-header
      :title="isEdit ? '编辑枚举' : '创建枚举'"
      @back="goBack"
    />

    <a-form
      :model="formState"
      :rules="rules"
      layout="vertical"
      ref="formRef"
    >
      <a-row :gutter="16">
        <a-col :span="12">
          <a-form-item label="枚举名称" name="name">
            <a-input v-model:value="formState.name" placeholder="请输入枚举名称" />
          </a-form-item>
        </a-col>
        <a-col :span="12">
          <a-form-item label="命名空间" name="namespace">
            <a-input v-model:value="formState.namespace" placeholder="请输入命名空间" />
          </a-form-item>
        </a-col>
      </a-row>

      <a-form-item label="描述" name="description">
        <a-textarea v-model:value="formState.description" placeholder="请输入枚举描述" :rows="2" />
      </a-form-item>

      <a-divider>枚举值列表</a-divider>

      <div class="value-operations">
        <a-button type="primary" @click="addValue">
          <plus-outlined /> 添加枚举值
        </a-button>
      </div>

      <a-table
        :columns="valueColumns"
        :data-source="formState.values"
        :pagination="false"
        row-key="tempId"
      >
        <template #bodyCell="{ column, record, index }">
          <template v-if="column.key === 'name'">
            <a-input v-model:value="record.name" placeholder="枚举值名称" />
          </template>
          <template v-else-if="column.key === 'value'">
            <a-input-number v-model:value="record.value" :min="0" style="width: 100%" />
          </template>
          <template v-else-if="column.key === 'description'">
            <a-input v-model:value="record.description" placeholder="枚举值描述" />
          </template>
          <template v-else-if="column.key === 'action'">
            <a-button type="link" danger @click="removeValue(index)">
              <delete-outlined /> 删除
            </a-button>
          </template>
        </template>
      </a-table>

      <div class="form-actions">
        <a-button @click="goBack">取消</a-button>
        <a-button type="primary" @click="submitForm" :loading="submitting">
          保存
        </a-button>
      </div>
    </a-form>
  </div>
</template>

<script setup>
import { ref, reactive, computed, onMounted } from 'vue';
import { useRouter, useRoute } from 'vue-router';
import { message } from 'ant-design-vue';
import { PlusOutlined, DeleteOutlined } from '@ant-design/icons-vue';
import api from '../services/api';

const router = useRouter();
const route = useRoute();
const formRef = ref(null);
const submitting = ref(false);

// 判断是编辑还是创建
const isEdit = computed(() => !!route.params.id);

// 表单状态
const formState = reactive({
  id: 0,
  name: '',
  namespace: 'Sample.Generated',
  description: '',
  values: []
});

// 表单验证规则
const rules = {
  name: [
    { required: true, message: '请输入枚举名称', trigger: 'blur' },
    { pattern: /^[A-Z][a-zA-Z0-9]*$/, message: '枚举名称必须以大写字母开头，且只能包含字母和数字', trigger: 'blur' }
  ],
  namespace: [
    { required: true, message: '请输入命名空间', trigger: 'blur' },
    { pattern: /^[a-zA-Z][a-zA-Z0-9.]*$/, message: '命名空间必须以字母开头，且只能包含字母、数字和点', trigger: 'blur' }
  ]
};

// 枚举值表格列定义
const valueColumns = [
  {
    title: '名称',
    key: 'name',
    width: '200px'
  },
  {
    title: '值',
    key: 'value',
    width: '120px'
  },
  {
    title: '描述',
    key: 'description'
  },
  {
    title: '操作',
    key: 'action',
    width: '100px',
    align: 'center'
  }
];

// 添加枚举值
let tempIdCounter = 0;
const addValue = () => {
  // 计算下一个可用的值
  let nextValue = 0;
  if (formState.values.length > 0) {
    const maxValue = Math.max(...formState.values.map(v => v.value || 0));
    nextValue = maxValue + 1;
  }

  formState.values.push({
    tempId: `temp_${tempIdCounter++}`,
    id: 0,
    name: '',
    value: nextValue,
    description: ''
  });
};

// 删除枚举值
const removeValue = (index) => {
  formState.values.splice(index, 1);
};

// 获取枚举详情
const fetchEnumDetail = async (id) => {
  try {
    const response = await api.enum.getById(id);
    const enumDef = response.data;
    
    // 更新表单状态
    formState.id = enumDef.id;
    formState.name = enumDef.name;
    formState.namespace = enumDef.namespace || 'Sample.Generated';
    formState.description = enumDef.description;
    
    // 处理枚举值，添加临时ID用于表格渲染
    formState.values = enumDef.values.map(val => ({
      ...val,
      tempId: `temp_${tempIdCounter++}`
    }));
  } catch (error) {
    message.error('获取枚举详情失败');
    console.error(error);
    goBack();
  }
};

// 提交表单
const submitForm = async () => {
  try {
    await formRef.value.validate();
    
    // 验证枚举值
    if (formState.values.length === 0) {
      message.warning('请至少添加一个枚举值');
      return;
    }
    
    // 验证枚举值名称
    for (const val of formState.values) {
      if (!val.name) {
        message.warning('枚举值名称不能为空');
        return;
      }
      if (!/^[A-Z][a-zA-Z0-9]*$/.test(val.name)) {
        message.warning(`枚举值名称 ${val.name} 必须以大写字母开头，且只能包含字母和数字`);
        return;
      }
    }
    
    // 验证枚举值唯一性
    const valueSet = new Set();
    const nameSet = new Set();
    for (const val of formState.values) {
      if (valueSet.has(val.value)) {
        message.warning(`枚举值 ${val.value} 重复`);
        return;
      }
      if (nameSet.has(val.name)) {
        message.warning(`枚举值名称 ${val.name} 重复`);
        return;
      }
      valueSet.add(val.value);
      nameSet.add(val.name);
    }
    
    submitting.value = true;
    
    // 准备提交的数据，移除临时ID
    const enumData = {
      ...formState,
      values: formState.values.map(({ tempId, ...val }) => val)
    };
    
    // 创建或更新枚举
    if (isEdit.value) {
      await api.enum.update(enumData.id, enumData);
      message.success('更新成功');
    } else {
      await api.enum.create(enumData);
      message.success('创建成功');
    }
    
    goBack();
  } catch (error) {
    console.error(error);
    message.error(isEdit.value ? '更新失败' : '创建失败');
  } finally {
    submitting.value = false;
  }
};

// 返回列表页
const goBack = () => {
  router.push('/enums');
};

onMounted(async () => {
  // 如果是编辑模式，获取枚举详情
  if (isEdit.value) {
    await fetchEnumDetail(route.params.id);
  } else {
    // 创建模式，默认添加一个枚举值
    addValue();
  }
});
</script>

<style scoped>
.value-operations {
  margin-bottom: 16px;
}

.form-actions {
  margin-top: 24px;
  display: flex;
  justify-content: flex-end;
  gap: 8px;
}
</style>